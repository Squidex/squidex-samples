// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.ImExport;

public sealed class Csv2SquidexConverter(string? fields)
{
    private readonly JsonMapping mapping = JsonMapping.ForCsv2Json(fields);

    public IEnumerable<DynamicData> ReadAll(CsvReader csvReader)
    {
        if (csvReader.Read())
        {
            csvReader.ReadHeader();

            while (csvReader.Read())
            {
                var data = new DynamicData();

                foreach (var (name, path, format) in mapping)
                {
                    if (csvReader.TryGetField<string>(name, out var value))
                    {
                        try
                        {
                            if (format == "raw")
                            {
                                SetValue(data, value, path);
                            }
                            else
                            {
                                SetValue(data, JToken.Parse(value!), path);
                            }
                        }
                        catch (JsonReaderException)
                        {
                            SetValue(data, JToken.Parse($"\"{value}\""), path);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot find field {name} in CSV file. Please check the delimiters setting.");
                    }
                }

                yield return data;
            }
        }
    }

    private static void SetValue(DynamicData data, JToken value, JsonPath path)
    {
        if (!data.TryGetValue(path[0].Key, out var property))
        {
            property = new JObject();

            data[path[0].Key] = property;
        }

        static object AddElement(object parent, string key, int index, JToken currentValue, bool merge)
        {
            if (index >= 0)
            {
                if (parent is JArray array)
                {
                    while (array.Count < index + 1)
                    {
                        array.Add(JValue.CreateNull());
                    }

                    if (merge && array[index].Type == currentValue.Type)
                    {
                        return array[index];
                    }

                    array[index] = currentValue;

                    return currentValue;
                }
            }
            else
            {
                if (parent is IDictionary<string, JToken> obj)
                {
                    if (merge && obj.TryGetValue(key, out var temp) && temp.Type == currentValue.Type)
                    {
                        return temp;
                    }

                    obj[key] = currentValue;

                    return currentValue;
                }
            }

            throw new CLIException("Invalid json mapping.");
        }

        object container = property;

        for (var i = 1; i < path.Count; i++)
        {
            var (key, index) = path[i];

            if (i == path.Count - 1)
            {
                AddElement(container, key, index, value, false);
            }
            else
            {
                var (_, next) = path[i + 1];

                if (next >= 0)
                {
                    container = AddElement(container, key, index, new JArray(), true);
                }
                else
                {
                    container = AddElement(container, key, index, new JObject(), true);
                }
            }
        }
    }
}
