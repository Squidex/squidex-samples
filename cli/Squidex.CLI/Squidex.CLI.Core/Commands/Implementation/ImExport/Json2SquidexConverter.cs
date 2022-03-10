// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.ImExport
{
    public sealed class Json2SquidexConverter
    {
        private readonly JsonMapping mapping;
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public Json2SquidexConverter(string? fields = null)
        {
            if (fields != null)
            {
                mapping = JsonMapping.ForCsv2Json(fields);
            }
        }

        public IEnumerable<DynamicData> ReadAll(JsonTextReader jsonReader)
        {
            if (jsonReader.Read() && jsonReader.TokenType == JsonToken.StartArray)
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.EndArray)
                    {
                        break;
                    }

                    var data = ReadOne(jsonReader);

                    yield return data;
                }
            }
        }

        public DynamicData ReadOne(JsonTextReader jsonReader)
        {
            var item = jsonSerializer.Deserialize<Dictionary<string, JToken>>(jsonReader);

            var data = new DynamicData();

            if (item == null)
            {
                return data;
            }

            if (mapping != null)
            {
                foreach (var (name, path, _) in mapping)
                {
                    if (item != null && item.TryGetValue(name, out var value))
                    {
                        try
                        {
                            SetValue(data, value, path);
                        }
                        catch (JsonReaderException)
                        {
                            SetValue(data, JToken.Parse($"\"{value}\""), path);
                        }
                    }
                }
            }
            else
            {
                foreach (var (key, value) in item)
                {
                    if (value is JObject obj)
                    {
                        data[key] = obj;
                    }
                    else
                    {
                        data[key] = new JObject
                        {
                            ["iv"] = value
                        };
                    }
                }
            }

            return data;
        }

        private static void SetValue(DynamicData data, JToken value, JsonPath path)
        {
            if (!data.TryGetValue(path[0].Key, out var property))
            {
                property = new JObject();

                data[path[0].Key] = property;
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

        private static object AddElement(object parent, string key, int index, JToken currentValue, bool merge)
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
    }
}
