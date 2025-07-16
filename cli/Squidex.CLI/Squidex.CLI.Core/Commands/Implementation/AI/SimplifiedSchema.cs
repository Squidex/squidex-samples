// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable MA0048 // File name must match type name

using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class SimplifiedSchema
{
    public string Name { get; set; }

    public string? Hints { get; set; }

    public SimplifiedField[] Fields { get; set; }
}

public sealed class SimplifiedField
{
    public string Name { get; set; }

    public string? Hints { get; set; }

    public SimplifiedFieldType Type { get; set; }

    public bool IsRequired { get; set; }

    public bool IsLocalized { get; set; }

    public int? MinLength { get; set; }

    public int? MaxLength { get; set; }

    public int? MinValue { get; set; }

    public int? MaxValue { get; set; }

    internal UpsertSchemaFieldDto ToField()
    {
        UpsertSchemaFieldDto ToStringField(StringFieldEditor editor)
        {
            return new UpsertSchemaFieldDto
            {
                Name = Name,
                Properties = new StringFieldPropertiesDto
                {
                    IsRequired = IsRequired,
                    MinLength = MinLength,
                    MaxLength = MaxLength,
                    Editor = editor,
                },
            };
        }

        switch (Type)
        {
            case SimplifiedFieldType.Boolean:
                return new UpsertSchemaFieldDto
                {
                    Name = Name,
                    Properties = new BooleanFieldPropertiesDto
                    {
                        IsRequired = IsRequired,
                    },
                };
            case SimplifiedFieldType.Image:
                return new UpsertSchemaFieldDto
                {
                    Name = Name,
                    Properties = new AssetsFieldPropertiesDto
                    {
                        IsRequired = IsRequired,
                    },
                };
            case SimplifiedFieldType.Number:
                return new UpsertSchemaFieldDto
                {
                    Name = Name,
                    Properties = new NumberFieldPropertiesDto
                    {
                        IsRequired = IsRequired,
                        MinValue = MinValue,
                        MaxValue = MaxValue,
                    },
                };
            case SimplifiedFieldType.Color:
                return ToStringField(StringFieldEditor.Color);
            case SimplifiedFieldType.Markdown:
                return ToStringField(StringFieldEditor.Markdown);
            case SimplifiedFieldType.MultilineText:
                return ToStringField(StringFieldEditor.TextArea);
            case SimplifiedFieldType.Slug:
                return ToStringField(StringFieldEditor.Slug);
            case SimplifiedFieldType.Text:
                return ToStringField(StringFieldEditor.Input);
            default:
                throw new NotSupportedException("Invalid field type.");
        }
    }
}

public enum SimplifiedFieldType
{
    Boolean,
    Color,
    Image,
    Markdown,
    MultilineText,
    Number,
    Slug,
    Text,
}
