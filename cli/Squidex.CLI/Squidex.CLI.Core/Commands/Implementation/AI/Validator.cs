// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Squidex.Text;

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed partial class Validator(SimplifiedSchema schema, HashSet<string> languages)
{
    private readonly Regex colorRegex = ColorRegex();
    private readonly List<string> errors = [];
    private readonly HashSet<string> fieldBlackList =
    [
        "approved",
        "approvedAt",
        "approvedBy",
        "approvedStatus",
        "created",
        "createdAt",
        "createdBy",
        "dueDate",
        "isDraft",
        "isPending",
        "isPublished",
        "isReady",
        "isReadyForPublish",
        "lastModified",
        "lastModifiedAt",
        "lastModifiedBy",
        "publishDate",
        "published",
        "rejected",
        "rejectedAt",
        "rejectedBy",
        "status",
        "submitted",
        "submittedAt",
        "submittedBy",
        "taskStatus",
        "updated",
        "updatedAt",
        "updatedBy",
        "visibility",
        "workflowState",
    ];

    private void AddError(string prefix, string message)
    {
        errors.Add($"{prefix}: {message}");
    }

    public IReadOnlyList<string> ValidateSchema()
    {
        ValidateSchemaCore();

        if (schema.Fields != null)
        {
            var index = 0;
            foreach (var field in schema.Fields)
            {
                ValidateSchemaField(field, $"fields[{index}]");
            }
        }

        return errors;
    }

    private void ValidateSchemaCore()
    {
        if (string.IsNullOrEmpty(schema.Name))
        {
            AddError("name", "Value is required");
        }
        else if (!schema.Name.IsSlug())
        {
            AddError("name", "Not a valid slug");
        }

        if (schema.Fields is not { Length: > 0 })
        {
            AddError("fields", "No field defined");
        }
    }

    private void ValidateSchemaField(SimplifiedField field, string prefix)
    {
        if (string.IsNullOrEmpty(field.Name))
        {
            AddError($"{prefix}.name", "Value is required");
        }
        else if (!BuildPropertyRegex().IsMatch(field.Name))
        {
            AddError($"{prefix}.name", "Not a property name");
        }

        if (fieldBlackList.Contains(field.Name))
        {
            AddError($"{prefix}.name", "Invalid metadata field");
        }

        if (field.MinValue > field.MaxValue)
        {
            AddError($"{prefix}.minValue", "Must be smaller than max value");
        }

        if (field.MinLength > field.MaxLength)
        {
            AddError($"{prefix}.minLength", "Must be smaller than max length");
        }
    }

    public IReadOnlyList<string> ValidateContents(List<Dictionary<string, JToken>> contents)
    {
        var index = 0;
        foreach (var content in contents)
        {
            ValidateContent(content, $"[{index}]");
            index++;
        }

        return errors;
    }

    private void ValidateContent(Dictionary<string, JToken> content, string prefix)
    {
        foreach (var field in schema.Fields)
        {
            var fieldPrefix = $"{prefix}.{field.Name}";

            content.TryGetValue(field.Name, out var value);
            ValidateField(value, field, fieldPrefix);
        }

        foreach (var key in content.Keys)
        {
            if (!schema.Fields.Any(x => x.Name == key))
            {
                var fieldPrefix = $"{prefix}.{key}";

                AddError(fieldPrefix, "Unknown field");
            }
        }
    }

    private void ValidateField(JToken? value, SimplifiedField field, string prefix)
    {
        if (value == null || value.Type == JTokenType.Null)
        {
            if (field.IsRequired)
            {
                AddError(prefix, "Value is required");
            }

            return;
        }

        if (field.IsLocalized)
        {
            try
            {
                if (value.Type != JTokenType.Object)
                {
                    AddError(prefix, $"Expected object, got {FormatType(value)}");
                    return;
                }

                var asObject = (JObject)value;

                foreach (var language in languages)
                {
                    var fieldPrefix = $"{prefix}.{language}";

                    asObject.TryGetValue(language, StringComparison.Ordinal, out var nestedValue);
                    ValidateValue(nestedValue, field, fieldPrefix);
                }

                foreach (var property in asObject.Properties())
                {
                    if (!languages.Contains(property.Name))
                    {
                        var fieldPrefix = $"{prefix}.{property.Name}";

                        AddError(fieldPrefix, "Unknown language");
                    }
                }
            }
            catch
            {
                AddError(prefix, $"Expected object, got invalid value");
            }
        }
        else
        {
            ValidateValue(value, field, prefix);
        }
    }

    private void ValidateValue(JToken? value, SimplifiedField field, string prefix)
    {
        if (value == null || value.Type == JTokenType.Null)
        {
            if (field.IsRequired)
            {
                AddError(prefix, "Value is required");
            }

            return;
        }

        switch (field.Type)
        {
            case SimplifiedFieldType.Boolean:
                ValidateBoolean(value, prefix);
                break;
            case SimplifiedFieldType.Color:
                ValidateColor(value, prefix);
                break;
            case SimplifiedFieldType.Image:
                ValidateImage(value, prefix);
                break;
            case SimplifiedFieldType.Text:
            case SimplifiedFieldType.Slug:
            case SimplifiedFieldType.Markdown:
            case SimplifiedFieldType.MultilineText:
                ValidateText(value, field, prefix);
                break;
            case SimplifiedFieldType.Number:
                ValidateNumber(value, field, prefix);
                break;
        }
    }

    private void ValidateNumber(JToken value, SimplifiedField field, string prefix)
    {
        if (value.Type is not JTokenType.Integer and not JTokenType.Float)
        {
            AddError(prefix, $"Expected number, got {FormatType(value)}");
            return;
        }

        var number = value.Value<double>();

        if (field.MinValue != null && number < field.MinValue)
        {
            AddError(prefix, $"Expected min value of {field.MinValue}, got {number}");
        }

        if (field.MaxValue != null && number > field.MaxValue)
        {
            AddError(prefix, $"Expected max value of {field.MaxValue}, got {number}");
        }
    }

    private void ValidateText(JToken value, SimplifiedField field, string prefix)
    {
        if (value.Type is not JTokenType.String)
        {
            AddError(prefix, $"Expected string, got {FormatType(value)}");
            return;
        }

        var text = value.Value<string>();
        if (text == null)
        {
            return;
        }

        if (field.MinLength != null && text.Length < field.MinLength)
        {
            AddError(prefix, $"Expected min length of {field.MinLength}, got {text.Length}");
        }

        if (field.MaxLength != null && text.Length > field.MaxLength)
        {
            AddError(prefix, $"Expected max length of {field.MaxLength}, got {text.Length}");
        }
    }

    private void ValidateColor(JToken value, string prefix)
    {
        if (value.Type is not JTokenType.String)
        {
            AddError(prefix, $"Expected string, got {FormatType(value)}");
            return;
        }

        var color = value.Value<string>();
        if (color != null && !colorRegex.IsMatch(color))
        {
            AddError(prefix, "Not a valid color");
        }
    }

    private void ValidateBoolean(JToken value, string prefix)
    {
        if (value.Type is not JTokenType.Boolean)
        {
            AddError(prefix, $"Expected boolean, got {FormatType(value)}");
            return;
        }
    }

    private void ValidateImage(JToken value, string prefix)
    {
        try
        {
            var image = value.ToObject<SimplifiedImage>();
            if (image == null)
            {
                AddError(prefix, "Expected object with description and file name, got invalid value");
                return;
            }

            if (string.IsNullOrWhiteSpace(image.Description))
            {
                AddError(prefix, "Image has no description");
            }

            if (string.IsNullOrWhiteSpace(image.FileName))
            {
                AddError(prefix, "Image has no file name");
            }
        }
        catch
        {
            AddError(prefix, "Expected object with description and file name, got invalid value");
        }
    }

    private static string FormatType(JToken value)
    {
        return value.Type.ToString().ToLowerInvariant();
    }

    [GeneratedRegex(@"#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})\b")]
    private static partial Regex ColorRegex();

    [GeneratedRegex("^[a-zA-Z0-9]+(\\-[a-zA-Z0-9]+)*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    private static partial Regex BuildPropertyRegex();
}
