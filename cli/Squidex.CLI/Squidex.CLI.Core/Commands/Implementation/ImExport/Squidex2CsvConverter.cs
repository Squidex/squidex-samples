// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.ImExport;

public sealed class Squidex2CsvConverter
{
    private readonly JsonMapping mapping;

    public IEnumerable<string> FieldNames
    {
        get { return mapping.Select(x => x.Name); }
    }

    public Squidex2CsvConverter(string? fields)
    {
        mapping = JsonMapping.ForJson2Csv(fields);
    }

    public IEnumerable<object?> GetValues(DynamicContent entity)
    {
        foreach (var (_, path, _) in mapping)
        {
            var value = GetValue(entity, path);

            switch (value)
            {
                case JValue jValue:
                    value = jValue.Value;
                    break;
                case JToken jToken:
                    value = jToken.ToString();
                    break;
            }

            if (value is string text)
            {
                yield return text.Replace("\n", "\\n", StringComparison.Ordinal);
            }
            else
            {
                yield return value;
            }
        }
    }

    private static object? GetValue(object? current, JsonPath path)
    {
        foreach (var (key, index) in path)
        {
            if (current is JObject obj)
            {
                if (obj.TryGetValue(key, StringComparison.Ordinal, out var temp))
                {
                    current = temp;
                }
                else
                {
                    return "<INVALID>";
                }
            }
            else if (current is IDictionary dict)
            {
                if (dict.Contains(key))
                {
                    current = dict[key];
                }
                else
                {
                    return "<INVALID>";
                }
            }
            else if (current is JArray arr)
            {
                if (index >= 0 && index < arr.Count)
                {
                    return arr[index];
                }
                else
                {
                    return "<INVALID>";
                }
            }
            else if (current != null)
            {
                var property = current.GetType().GetProperties().FirstOrDefault(x => x.CanRead && string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    current = property.GetValue(current);
                }
                else
                {
                    return "<INVALID>";
                }
            }
        }

        if (current is JValue value)
        {
            return value.Value;
        }
        else if (current?.GetType().IsClass == true)
        {
            return current.JsonString();
        }
        else
        {
            return current;
        }
    }
}
