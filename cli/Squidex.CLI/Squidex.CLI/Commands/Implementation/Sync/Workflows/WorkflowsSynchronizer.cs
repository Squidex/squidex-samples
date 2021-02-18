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
        private readonly ILogger log;

        public string Name => "Workflow";

        public WorkflowsSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var current = await session.Apps.GetWorkflowsAsync(session.App);

            var index = 0;

            foreach (var workflow in current.Items.OrderBy(x => x.Name))
            {
                var workflowName = workflow.Name;

                await log.DoSafeAsync($"Exporting '{workflowName}' ({workflow.Id})", async () =>
                {
                    await jsonHelper.WriteWithSchemaAs<UpdateWorkflowDto>(directoryInfo, $"workflows/workflow{index}.json", workflow, "../__json/workflow");
                });

                index++;
            }
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var newWorkflows = GetWorkflowModels(directoryInfo, jsonHelper).ToList();

            if (!newWorkflows.HasDistinctNames(x => x.Name))
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

            if (options.NoDeletion)
            {
                foreach (var (name, workflow) in workflowsByName.ToList())
                {
                    if (newWorkflows.All(x => x.Name == name))
                    {
                        await log.DoSafeAsync($"Workflow '{name}' deleting", async () =>
                        {
                            await session.Apps.DeleteWorkflowAsync(session.App, workflow.Id);

                            workflowsByName.Remove(name);
                        });
                    }
                }
            }

            foreach (var newWorkflow in newWorkflows)
            {
                if (workflowsByName.ContainsKey(newWorkflow.Name))
                {
                    continue;
                }

                await log.DoSafeAsync($"Workflow '{newWorkflow.Name}' creating", async () =>
                {
                    if (workflowsByName.ContainsKey(newWorkflow.Name))
                    {
                        throw new CLIException("Name already used.");
                    }

                    var request = new AddWorkflowDto
                    {
                        Name = newWorkflow.Name
                    };

                    var created = await session.Apps.PostWorkflowAsync(session.App, request);

                    workflowsByName[newWorkflow.Name] = created.Items.FirstOrDefault(x => x.Name == newWorkflow.Name);
                });
            }

            foreach (var newWorkflow in newWorkflows)
            {
                var workflow = workflowsByName.GetValueOrDefault(newWorkflow.Name);

                if (workflow == null)
                {
                    return;
                }

                await log.DoSafeAsync($"Workflow '{newWorkflow.Name}' updating", async () =>
                {
                    await session.Apps.PutWorkflowAsync(session.App, workflow.Id, newWorkflow);
                });
            }
        }

        private IEnumerable<UpdateWorkflowDto> GetWorkflowModels(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            foreach (var file in directoryInfo.GetFiles("workflows/*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    var workflow = jsonHelper.Read<UpdateWorkflowDto>(file, log);

                    yield return workflow;
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

            await jsonHelper.WriteWithSchema(directoryInfo, "workflows/__workflow.json", sample, "../__json/workflow");
        }
    }
}
