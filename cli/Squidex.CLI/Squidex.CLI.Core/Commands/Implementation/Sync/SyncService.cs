// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using NJsonSchema.Infrastructure;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public sealed class SyncService : ISyncService
    {
        private readonly JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings;
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly JsonSerializer jsonSerializer;

        public IFileSystem FileSystem { get; }

        public FolderTree Folders { get; }

        internal sealed class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
            {
                var contract = base.CreateDictionaryContract(objectType);

                contract.DictionaryKeyResolver = propertyName => propertyName;

                return contract;
            }
        }

        public SyncService(IFileSystem fileSystem, ISession session)
        {
            Folders = new FolderTree(session);

            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCaseExceptDictionaryKeysResolver()
            };

            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            jsonSchemaGeneratorSettings = new JsonSchemaGeneratorSettings
            {
                FlattenInheritanceHierarchy = true,
                SchemaType = SchemaType.JsonSchema,
                SchemaNameGenerator = new DefaultSchemaNameGenerator(),
                SerializerSettings = jsonSerializerSettings
            };

            jsonSchemaGeneratorSettings.SchemaProcessors.Add(new InheritanceProcessor());
            jsonSchemaGeneratorSettings.SchemaProcessors.Add(new GuidFixProcessor());

            jsonSchemaGeneratorSettings.TypeMappers.Add(
                new PrimitiveTypeMapper(typeof(DynamicData), schema =>
                {
                    schema.Type = JsonObjectType.Object;

                    schema.AdditionalPropertiesSchema = new JsonSchema
                    {
                        Description = "Any."
                    };
                }));

            jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);

            FileSystem = fileSystem;
        }

        public T Read<T>(IFile file, ILogger log)
        {
            var jsonText = file.ReadAllText();
            var jsonSchema = GetSchema<T>();

            var errors = jsonSchema.Validate(jsonText);

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

            return JsonConvert.DeserializeObject<T>(jsonText, jsonSerializerSettings)!;
        }

        public async Task WriteWithSchemaAs<T>(IFile file, object sample, string schema) where T : class
        {
            await using (var stream = file.OpenWrite())
            {
                Write(Convert<T>(sample), stream, $"./{schema}.json");
            }
        }

        public async Task WriteWithSchema<T>(IFile file, T sample, string schema) where T : class
        {
            await using (var stream = file.OpenWrite())
            {
                Write(sample, stream, $"./{schema}.json");
            }
        }

        public async Task WriteJsonSchemaAsync<T>(IFile file)
        {
            await using (var stream = file.OpenWrite())
            {
                await using (var textWriter = new StreamWriter(stream))
                {
                    var jsonSchema = GetSchema<T>();
                    var jsonSchemaType = jsonSchemaGeneratorSettings.SchemaType;

                    var json = JsonSchemaSerialization.ToJson(
                        jsonSchema,
                        jsonSchemaType,
                        JsonSchema.CreateJsonSerializerContractResolver(jsonSchemaType),
                        Formatting.Indented);

                    await textWriter.WriteAsync(json);
                }
            }
        }

        public void Write<T>(T value, Stream stream, string? schemaRef = null) where T : class
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
                return (T)jsonSerializer.Deserialize(reader, typeof(T))!;
            }
        }
    }
}
