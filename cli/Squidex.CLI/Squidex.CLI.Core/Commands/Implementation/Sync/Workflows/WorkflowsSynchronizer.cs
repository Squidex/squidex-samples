// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Workflows;

public sealed class WorkflowsSynchronizer : ISynchronizer
{
    private const string Ref = "../__json/workflow";
    private readonly ILogger log;

    public string Name => "Workflows";

    public string Description => "Synchronizes all workflows from the app settings.";

    public WorkflowsSynchronizer(ILogger log)
    {
        this.log = log;
    }

    public Task CleanupAsync(IFileSystem fs)
    {
        foreach (var file in GetFiles(fs))
        {
            file.Delete();
        }

        return Task.CompletedTask;
    }

    public async Task ExportAsync(ISyncService sync, SyncOptions options, ISession session)
    {
        var current = await session.Client.Apps.GetWorkflowsAsync();

        var schemas = await session.Client.Schemas.GetSchemasAsync();
        var schemaMap = schemas.Items.ToDictionary(x => x.Id, x => x.Name);

        await current.Items.OrderBy(x => x.Name).Foreach(async (workflow, i) =>
        {
            var workflowName = workflow.Name;

            MapSchemas(workflow, schemaMap);

            await log.DoSafeAsync($"Exporting '{workflowName}' ({workflow.Id})", async () =>
            {
                await sync.WriteWithSchemaAs<UpdateWorkflowDto>(new FilePath($"workflows", $"workflow{i}.json"), workflow, Ref);
            });
        });
    }

    public Task DescribeAsync(ISyncService sync, MarkdownWriter writer)
    {
        var models =
            GetFiles(sync.FileSystem)
                .Select(x => sync.Read<UpdateWorkflowDto>(x, log))
                .ToList();

        writer.Paragraph($"{models.Count} workflow(s).");

        if (models.Count > 0)
        {
            var rows = models.Select(x => new object[] { x.Name, string.Join(", ", x.Steps.Select(y => y.Key)), x.Initial }).OrderBy(x => x[0]).ToArray();

            writer.Table(new[] { "Name", "Steps", "Initial" }, rows);
        }

        return Task.CompletedTask;
    }

    public async Task ImportAsync(ISyncService sync, SyncOptions options, ISession session)
    {
        var models =
            GetFiles(sync.FileSystem)
                .Select(x => sync.Read<UpdateWorkflowDto>(x, log))
                .ToList();

        if (!models.HasDistinctNames(x => x.Name))
        {
            log.WriteLine("ERROR: Can only sync workflows when all target workflows have distinct names.");
            return;
        }

        var current = await session.Client.Apps.GetWorkflowsAsync();

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
                        await session.Client.Apps.DeleteWorkflowAsync(workflow.Id);

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

                var created = await session.Client.Apps.PostWorkflowAsync(request);

                workflowsByName[workflow.Name] = created.Items.Find(x => x.Name == workflow.Name);
            });
        }

        var schemas = await session.Client.Schemas.GetSchemasAsync();
        var schemaMap = schemas.Items.ToDictionary(x => x.Name, x => x.Id);

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
                await session.Client.Apps.PutWorkflowAsync(existing.Id, workflow);
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

    private static IEnumerable<IFile> GetFiles(IFileSystem fs)
    {
        foreach (var file in fs.GetFiles(new FilePath("workflows"), ".json"))
        {
            if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
            {
                yield return file;
            }
        }
    }

    public async Task GenerateSchemaAsync(ISyncService sync)
    {
        await sync.WriteJsonSchemaAsync<UpdateWorkflowDto>(new FilePath("workflow.json"));

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

        await sync.WriteWithSchema(new FilePath("workflows", "__workflow.json"), sample, Ref);
    }
}
