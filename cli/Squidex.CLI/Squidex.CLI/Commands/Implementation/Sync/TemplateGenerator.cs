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
            await WriteWorkflowAsync();
        }

        private async Task WriteAppAsync()
        {
            await WriteJsonSchemaAsync<AppSettings>("app.json");

            var sample = new AppSettings
            {
                Roles = new Dictionary<string, AppRoleSetting>
                {
                    ["custom"] = new AppRoleSetting
                    {
                        Permissions = new string[]
                        {
                            "schemas.*"
                        }
                    }
                },
                Clients = new Dictionary<string, AppClientSetting>
                {
                    ["test"] = new AppClientSetting
                    {
                        Role = "Owner"
                    }
                },
                Languages = new Dictionary<string, UpdateLanguageDto>
                {
                    ["en"] = new UpdateLanguageDto
                    {
                        IsMaster = true
                    }
                }
            };

            await WriteSampleAsync("__app.json", sample, "__json/app");
        }

        private async Task WriteSchemaAsync()
        {
            await WriteJsonSchemaAsync<SchemaSettings>("schema.json");

            var sample = new SchemaSettings
            {
                Name = "my-schema",
                Schema = new SynchronizeSchemaDto
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
                }
            };

            await WriteSampleAsync("schemas/__schema.json", sample, "../__json/schema");
        }

        private async Task WriteWorkflowAsync()
        {
            await WriteJsonSchemaAsync<UpdateWorkflowDto>("workflow.json");

            var sample = new UpdateWorkflowDto
            {
                Name = "my-workflow",
                Steps = new Dictionary<string, WorkflowStepDto>
                {
                    ["Draft"] = new WorkflowStepDto
                    {
                        Color = "#ff0000",
                        Transitions = new Dictionary<string, WorkflowTransitionDto>
                        {
                            ["Published"] = new WorkflowTransitionDto()
                        }
                    },
                    ["Published"] = new WorkflowStepDto
                    {
                        Color = "#00ff00",
                        Transitions = new Dictionary<string, WorkflowTransitionDto>
                        {
                            ["Draft"] = new WorkflowTransitionDto()
                        },
                        NoUpdate = true,
                    }
                }
            };

            await WriteSampleAsync("workflows/__workflow.json", sample, "../__json/workflow");
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
