// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public class JsonHelper
    {
        private readonly JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings;
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly JsonSerializer jsonSerializer;
        private readonly JsonSchemaIdConverter schemaIdConverter = new JsonSchemaIdConverter();

        internal sealed class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
            {
                JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);

                contract.DictionaryKeyResolver = propertyName => propertyName;

                return contract;
            }
        }

        public JsonHelper()
        {
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCaseExceptDictionaryKeysResolver()
            };

            jsonSerializerSettings.Converters.Add(schemaIdConverter);
            jsonSerializerSettings.Formatting = Formatting.Indented;

            jsonSchemaGeneratorSettings = new JsonSchemaGeneratorSettings
            {
                SchemaType = SchemaType.JsonSchema,
                SerializerSettings = jsonSerializerSettings,
                FlattenInheritanceHierarchy = true
            };

            jsonSchemaGeneratorSettings.SchemaProcessors.Add(new InheritanceProcessor());
            jsonSchemaGeneratorSettings.SchemaProcessors.Add(new GuidFixProcessor());

            jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
        }

        internal sealed class JsonSchemaIdConverter : JsonConverter<string>
        {
            public Dictionary<string, string> SchemaMap { get; set; } = new Dictionary<string, string>();

            public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType != JsonToken.String)
                {
                    throw new JsonException("Expected string.");
                }

                var value = (string)reader.Value;

                if (!SchemaMap.TryGetValue(value, out var id))
                {
                    throw new JsonException($"Schema {value} not found.");
                }

                return id;
            }

            public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
            {
                var schemaName = SchemaMap.FirstOrDefault(x => x.Value == value).Key;

                if (string.IsNullOrWhiteSpace(schemaName))
                {
                    writer.WriteValue("<NOT FOUND>");
                }
                else
                {
                    writer.WriteValue(schemaName);
                }
            }
        }

        public void SetSchemaMap(Dictionary<string, string> schemas)
        {
            schemaIdConverter.SchemaMap = schemas;
        }

        public T Read<T>(FileInfo file, ILogger log)
        {
            var json = File.ReadAllText(file.FullName);

            var schema = GetSchema<T>();

            var errors = schema.Validate(json);

            if (errors.Any())
            {
                log.WriteLine("File {0} is not valid", file.FullName);

                foreach (var error in errors)
                {
                    if (error.HasLineInfo)
                    {
                        log.WriteLine("* {0}, Line: {1}, Col: {2}", error, error.LineNumber, error.LinePosition);
                    }
                    else
                    {
                        log.WriteLine("* {0}", error);
                    }
                }

                throw new JsonException($"Error reading file {file.FullName}");
            }

            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
        }

        public void WriteSchema<T>(Stream stream)
        {
            using (var textWriter = new StreamWriter(stream))
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    var obj = GetSchema<T>();

                    jsonSerializer.Serialize(jsonWriter, obj);
                }
            }
        }

        public void WriteAs<T>(object value, Stream stream, string schemaRef = null) where T : class
        {
            Write(Convert<T>(value), stream, schemaRef);
        }

        public void Write<T>(T value, Stream stream, string schemaRef = null) where T : class
        {
            using (var textWriter = new StreamWriter(stream))
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    if (schemaRef != null)
                    {
                        var withSchema = new JObject
                        {
                            ["$schema"] = schemaRef
                        };

                        foreach (var (key, v) in JObject.FromObject(value, jsonSerializer))
                        {
                            withSchema[key] = v;
                        }

                        jsonSerializer.Serialize(jsonWriter, withSchema);
                    }
                    else
                    {
                        jsonSerializer.Serialize(jsonWriter, value);
                    }
                }
            }
        }

        private JsonSchema GetSchema<T>()
        {
            var schema = JsonSchema.FromType<T>(jsonSchemaGeneratorSettings);

            schema.AllowAdditionalProperties = true;

            return schema;
        }

        public T Convert<T>(object value)
        {
            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }

            var memoryStream = new MemoryStream();

            using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
            {
                jsonSerializer.Serialize(writer, value);
            }

            memoryStream.Position = 0;

            using (var reader = new StreamReader(memoryStream))
            {
                return (T)jsonSerializer.Deserialize(reader, typeof(T));
            }
        }
    }
}
