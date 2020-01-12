// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation
{
    public sealed class Json2SquidexConverter
    {
        private readonly JsonMapping mapping = new JsonMapping();
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public Json2SquidexConverter(string fields)
        {
            mapping = JsonMapping.ForCsv2Json(fields);
        }

        public IEnumerable<DummyData> ReadAll(JsonTextReader jsonReader)
        {
            if (jsonReader.Read() && jsonReader.TokenType == JsonToken.StartArray)
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.EndArray)
                    {
                        break;
                    }

                    var item = jsonSerializer.Deserialize<Dictionary<string, JToken>>(jsonReader);

                    var data = new DummyData();

                    foreach (var field in mapping)
                    {
                        if (item.TryGetValue(field.Name, out var value))
                        {
                            try
                            {
                                SetValue(data, value, field.Path);
                            }
                            catch (JsonReaderException)
                            {
                                SetValue(data, JToken.Parse($"\"{value}\""), field.Path);
                            }
                        }
                    }

                    yield return data;
                }
            }
        }

        private void SetValue(DummyData data, JToken value, JsonPath path)
        {
            if (!data.TryGetValue(path[0].Key, out var property))
            {
                property = new Dictionary<string, JToken>();

                data[path[0].Key] = property;
            }

            object AddElement(object parent, string key, int index, JToken currentValue, bool merge)
            {
                if (index >= 0)
                {
                    if (parent is JArray array)
                    {
                        while (array.Count < index + 1)
                        {
                            array.Add(null);
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

                throw new SquidexException("Invalid json mapping.");
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
}
