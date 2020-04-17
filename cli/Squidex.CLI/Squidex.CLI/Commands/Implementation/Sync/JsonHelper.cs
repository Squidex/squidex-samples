// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public static class JsonHelper
    {
        private static readonly JsonSchemaGeneratorSettings JsonSchemaGeneratorSettings;
        private static readonly JsonSerializerSettings JsonSerializerSettings;
        private static readonly JsonSerializer JsonSerializer;
        private static readonly JsonSchemaIdConverter SchemaIdConverter = new JsonSchemaIdConverter();

        static JsonHelper()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            JsonSerializerSettings.Converters.Add(SchemaIdConverter);

            JsonSchemaGeneratorSettings = new JsonSchemaGeneratorSettings
            {
                SerializerSettings = JsonSerializerSettings
            };

            JsonSerializer = JsonSerializer.Create(JsonSerializerSettings);
        }

        internal sealed class JsonSchemaIdConverter : JsonConverter<Guid>
        {
            public Dictionary<string, Guid> SchemaMap { get; set; } = new Dictionary<string, Guid>();

            public override Guid ReadJson(JsonReader reader, Type objectType, [AllowNull] Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
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

            public override void WriteJson(JsonWriter writer, [AllowNull] Guid value, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }
        }

        public static void SetSchemaMap(Dictionary<string, Guid> schemas)
        {
            SchemaIdConverter.SchemaMap = schemas;
        }

        public static T Read<T>(FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        return JsonSerializer.Deserialize<T>(jsonReader);
                    }
                }
            }
        }

        public static T Read<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        public static string SampleJson<T>(T sample, string schemaRef)
        {
            var obj = new JObject
            {
                ["$schema"] = schemaRef
            };

            foreach (var (key, value) in JObject.FromObject(sample, JsonSerializer))
            {
                obj[key] = value;
            }

            var json = obj.ToString(Formatting.Indented);

            return json;
        }

        public static string SchemaString<T>()
        {
            var schema = JsonSchema.FromType<T>(JsonSchemaGeneratorSettings);

            schema.AllowAdditionalProperties = true;

            return schema.ToJson();
        }
    }
}
