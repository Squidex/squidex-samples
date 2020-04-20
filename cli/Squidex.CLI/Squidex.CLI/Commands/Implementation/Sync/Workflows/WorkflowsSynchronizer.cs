// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Workflows
{
    public sealed class WorkflowsSynchronizer : ISynchronizer
    {
        public Task SynchronizeAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            return Task.CompletedTask;
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
                        NoUpdate = true,
                    }
                }
            };

            await jsonHelper.WriteSampleAsync(directoryInfo, "workflows/__workflow.json", sample, "../__json/workflow");
        }
    }
}
