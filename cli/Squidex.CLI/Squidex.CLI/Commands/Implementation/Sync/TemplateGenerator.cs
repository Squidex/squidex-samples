// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Squidex.CLI.Commands.Implementation.Sync.Model;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public class TemplateGenerator
    {
        private readonly DirectoryInfo directoryInfo;

        public TemplateGenerator(string path)
        {
            directoryInfo = Directory.CreateDirectory(path);
        }

        public async Task GenerateAsync()
        {
            await WriteAppAsync();
            await WriteSchemaAsync();
        }

        private async Task WriteSchemaAsync()
        {
            await WriteJsonSchemaAsync<UpsertSchemaDto>("schema.json");

            var sample = new SynchronizeSchemaDto
            {
                Properties = new SchemaPropertiesDto
                {
                    Label = "My Schema"
                },
                Fields = new List<UpsertSchemaFieldDto>
                {
                    new UpsertSchemaFieldDto
                    {
                        Name = "my-string",
                        Properties = new StringFieldPropertiesDto
                        {
                            IsRequired = true
                        },
                        Partitioning = "invariant"
                    }
                },
                IsPublished = true
            };

            await WriteSampleAsync("schemas/__schema.json", sample, "../__json/schema");
        }

        private async Task WriteAppAsync()
        {
            await WriteJsonSchemaAsync<AppSettings>("app.json");

            var sample = new AppSettings
            {
                Contributors = new Dictionary<string, ContributorSetting>
                {
                    ["mail@squidex.io"] = new ContributorSetting
                    {
                        Role = "Owner"
                    }
                },
                Clients = new Dictionary<string, ClientSetting>
                {
                    ["test"] = new ClientSetting
                    {
                        Role = "Owner"
                    }
                }
            };

            await WriteSampleAsync("__app.json", sample, "__json/app");
        }

        private async Task WriteSampleAsync<T>(string path, T sample, string schema)
        {
            var fileInfo = GetFile(path);

            var json = JsonHelper.SampleJson(sample, $"./{schema}.json");

            await File.WriteAllTextAsync(fileInfo.FullName, json);
        }

        private async Task WriteJsonSchemaAsync<T>(string path)
        {
            var fileInfo = GetFile(Path.Combine("__json", path));

            var json = JsonHelper.SchemaString<T>();

            await File.WriteAllTextAsync(fileInfo.FullName, json);
        }

        private FileInfo GetFile(string path)
        {
            var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, path));

            if (!fileInfo.Directory.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            return fileInfo;
        }
    }
}
