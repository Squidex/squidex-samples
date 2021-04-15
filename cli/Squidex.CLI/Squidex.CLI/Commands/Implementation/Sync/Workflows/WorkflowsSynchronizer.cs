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
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Workflows
{
    public sealed class WorkflowsSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/workflow";
        private readonly ILogger log;

        public string Name => "Workflow";

        public WorkflowsSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task CleanupAsync(DirectoryInfo directoryInfo)
        {
            foreach (var file in GetFiles(directoryInfo))
            {
                file.Delete();
            }

            return Task.CompletedTask;
        }

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetWorkflowsAsync(session.App);

            var schemas = await session.Schemas.GetSchemasAsync(session.App);
            var schemaMap = schemas.Items.ToDictionary(x => x.Id, x => x.Name);

            await current.Items.OrderBy(x => x.Name).Foreach(async (workflow, i) =>
            {
                var workflowName = workflow.Name;

                MapSchemas(workflow, schemaMap);

                await log.DoSafeAsync($"Exporting '{workflowName}' ({workflow.Id})", async () =>
                {
                    await jsonHelper.WriteWithSchemaAs<UpdateWorkflowDto>(directoryInfo, $"workflows/workflow{i}.json", workflow, Ref);
                });
            });
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var models =
                GetFiles(directoryInfo)
                    .Select(x => jsonHelper.Read<UpdateWorkflowDto>(x, log))
                    .ToList();

            if (!models.HasDistinctNames(x => x.Name))
            {
                log.WriteLine("ERROR: Can only sync workflows when all target workflows have distinct names.");
                return;
            }

            var current = await session.Apps.GetWorkflowsAsync(session.App);

            if (!current.Items.HasDistinctNames(x => x.Name))
            {
                log.WriteLine("ERROR: Can only sync workflows when all current workflows have distinct names.");
                return;
            }

            var workflowsByName = current.Items.ToDictionary(x => x.Name);

            if (options.Delete)
            {
                foreach (var (name, workflow) in workflowsByName.ToList())
                {
                    if (models.All(x => x.Name == name))
                    {
                        await log.DoSafeAsync($"Workflow '{name}' deleting", async () =>
                        {
                            await session.Apps.DeleteWorkflowAsync(session.App, workflow.Id);

                            workflowsByName.Remove(name);
                        });
                    }
                }
            }

            foreach (var workflow in models)
            {
                if (workflowsByName.ContainsKey(workflow.Name))
                {
                    continue;
                }

                await log.DoSafeAsync($"Workflow '{workflow.Name}' creating", async () =>
                {
                    if (workflowsByName.ContainsKey(workflow.Name))
                    {
                        throw new CLIException("Name already used.");
                    }

                    var request = new AddWorkflowDto
                    {
                        Name = workflow.Name
                    };

                    var created = await session.Apps.PostWorkflowAsync(session.App, request);

                    workflowsByName[workflow.Name] = created.Items.FirstOrDefault(x => x.Name == workflow.Name);
                });
            }

            var schemas = await session.Schemas.GetSchemasAsync(session.App);
            var schemaMap = schemas.Items.ToDictionary(x => x.Id, x => x.Name);

            foreach (var workflow in models)
            {
                var existing = workflowsByName.GetValueOrDefault(workflow.Name);

                if (existing == null)
                {
                    return;
                }

                MapSchemas(workflow, schemaMap);

                await log.DoSafeAsync($"Workflow '{workflow.Name}' updating", async () =>
                {
                    await session.Apps.PutWorkflowAsync(session.App, existing.Id, workflow);
                });
            }
        }

        private void MapSchemas(WorkflowDto workflow, Dictionary<string, string> schemaMap)
        {
            var schemaIds = new List<string>();

            foreach (var schema in workflow.SchemaIds)
            {
                if (!schemaMap.TryGetValue(schema, out var found))
                {
                    log.WriteLine($"Schema {schema} not found.");

                    schemaIds.Add(schema);
                }
                else
                {
                    schemaIds.Add(found);
                }
            }

            workflow.SchemaIds = schemaIds;
        }

        private void MapSchemas(UpdateWorkflowDto workflow, Dictionary<string, string> schemaMap)
        {
            var schemaIds = new List<string>();

            foreach (var schema in workflow.SchemaIds)
            {
                if (!schemaMap.TryGetValue(schema, out var found))
                {
                    log.WriteLine($"Schema {schema} not found.");

                    schemaIds.Add(schema);
                }
                else
                {
                    schemaIds.Add(found);
                }
            }

            workflow.SchemaIds = schemaIds;
        }

        private static IEnumerable<FileInfo> GetFiles(DirectoryInfo directoryInfo)
        {
            foreach (var file in directoryInfo.GetFiles("workflows/*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<UpdateWorkflowDto>(directoryInfo, "workflow.json");

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
                        NoUpdate = true
                    }
                },
                Initial = "Draft"
            };

            await jsonHelper.WriteWithSchema(directoryInfo, "workflows/__workflow.json", sample, Ref);
        }
    }
}
