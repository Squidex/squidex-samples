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

        internal sealed class JsonSchemaIdConverter : JsonConverter<Guid>
        {
            public Dictionary<string, Guid> SchemaMap { get; set; } = new Dictionary<string, Guid>();

            public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
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

            public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }
        }

        public void SetSchemaMap(Dictionary<string, Guid> schemas)
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

        public string SampleJson<T>(T sample, string schemaRef)
        {
            var obj = new JObject
            {
                ["$schema"] = schemaRef
            };

            foreach (var (key, value) in JObject.FromObject(sample, jsonSerializer))
            {
                obj[key] = value;
            }

            var json = obj.ToString(Formatting.Indented);

            return json;
        }

        public string SchemaString<T>()
        {
            var schema = GetSchema<T>();

            return schema.ToJson();
        }

        private JsonSchema GetSchema<T>()
        {
            var schema = JsonSchema.FromType<T>(jsonSchemaGeneratorSettings);

            schema.AllowAdditionalProperties = true;

            return schema;
        }
    }
}
