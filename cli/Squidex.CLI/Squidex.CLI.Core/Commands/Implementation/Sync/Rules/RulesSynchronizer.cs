// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Rules;

public sealed class RulesSynchronizer : ISynchronizer
{
    private const string Ref = "../__json/rule";
    private readonly ILogger log;

    public string Name => "Rules";

    public string Description => "Synchronizes all rules, but not rule events.";

    public RulesSynchronizer(ILogger log)
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
        var current = await session.Client.ExtendableRules.GetRulesAsync();

        await MapSchemaIdsToNamesAsync(session, current);

        await current.Items.OrderBy(x => x.Created).Foreach(async (rule, i) =>
        {
            var ruleName = rule.Name;

            if (string.IsNullOrWhiteSpace(ruleName))
            {
                ruleName = "<Unnammed>";
            }

            await log.DoSafeAsync($"Exporting {ruleName} ({rule.Id})", async () =>
            {
                await sync.WriteWithSchemaAs<RuleModel>(new FilePath("rules", $"rule{i}.json"), rule, Ref);
            });
        });
    }

    public Task DescribeAsync(ISyncService sync, MarkdownWriter writer)
    {
        var models =
            GetFiles(sync.FileSystem)
                .Select(x => sync.Read<RuleModel>(x, log))
                .ToList();

        writer.Paragraph($"{models.Count} rule(s).");

        if (models.Count > 0)
        {
            var rows = models.Select(x => new object[] { x.Name, x.Trigger.TypeName(), x.Action.TypeName() }).OrderBy(x => x[0]).ToArray();

            writer.Table(["Name", "Trigger", "Action"], rows);
        }

        return Task.CompletedTask;
    }

    public async Task ImportAsync(ISyncService sync, SyncOptions options, ISession session)
    {
        var models =
            GetFiles(sync.FileSystem)
                .Select(x => sync.Read<RuleModel>(x, log))
                .ToList();

        if (!models.HasDistinctNames(x => x.Name))
        {
            log.WriteLine("ERROR: Can only sync rules when all target rules have distinct names.");
            return;
        }

        var current = await session.Client.ExtendableRules.GetRulesAsync();

        if (!current.Items.HasDistinctNames(x => x.Name))
        {
            log.WriteLine("ERROR: Can only sync rules when all current rules have distinct names.");
            return;
        }

        var rulesByName = current.Items.ToDictionary(x => x.Name);

        if (options.Delete)
        {
            foreach (var (name, rule) in rulesByName.ToList())
            {
                if (models.TrueForAll(x => x.Name != name))
                {
                    await log.DoSafeAsync($"Rule '{name}' deleting", async () =>
                    {
                        await session.Client.ExtendableRules.DeleteRuleAsync(rule.Id);

                        rulesByName.Remove(name);
                    });
                }
            }
        }

        await MapSchemaNamesToIdsAsync(session, models);

        foreach (var newRule in models)
        {
            if (rulesByName.ContainsKey(newRule.Name))
            {
                continue;
            }

            await log.DoSafeAsync($"Rule '{newRule.Name}' creating", async () =>
            {
                if (rulesByName.ContainsKey(newRule.Name))
                {
                    throw new CLIException("Name already used.");
                }

                var request = newRule.ToCreate();

                var created = await session.Client.ExtendableRules.CreateRuleAsync(request);

                rulesByName[newRule.Name] = created;
            });
        }

        foreach (var newRule in models)
        {
            var rule = rulesByName.GetValueOrDefault(newRule.Name);

            if (rule == null)
            {
                return;
            }

            await log.DoVersionedAsync($"Rule '{newRule.Name}' updating", rule.Version, async () =>
            {
                var request = newRule.ToUpdate();

                rule = await session.Client.ExtendableRules.UpdateRuleAsync(rule.Id, request);

                return rule.Version;
            });

            if (newRule.IsEnabled != rule.IsEnabled)
            {
                if (newRule.IsEnabled)
                {
                    await log.DoVersionedAsync($"Rule '{newRule.Name}' enabling", rule.Version, async () =>
                    {
                        var result = await session.Client.ExtendableRules.EnableRuleAsync(rule.Id);

                        return result.Version;
                    });
                }
                else
                {
                    await log.DoVersionedAsync($"Rule '{newRule.Name}' disabling", rule.Version, async () =>
                    {
                        var result = await session.Client.ExtendableRules.DisableRuleAsync(rule.Id);

                        return result.Version;
                    });
                }
            }
        }
    }

    private async Task MapSchemaIdsToNamesAsync(ISession session, ExtendableRulesDto current)
    {
        var schemas = await session.Client.Schemas.GetSchemasAsync();

        var map = schemas.Items.ToDictionary(x => x.Id, x => x.Name);

        foreach (var rule in current.Items)
        {
            if (rule.Trigger is ContentChangedRuleTriggerDto contentTrigger)
            {
                MapSchemas(contentTrigger.Schemas, map);
                MapSchemas(contentTrigger.ReferencedSchemas, map);
            }
        }
    }

    private async Task MapSchemaNamesToIdsAsync(ISession session, List<RuleModel> models)
    {
        var schemas = await session.Client.Schemas.GetSchemasAsync();

        var map = schemas.Items.ToDictionary(x => x.Name, x => x.Id);

        foreach (var newRule in models)
        {
            if (newRule.Trigger is ContentChangedRuleTriggerDto contentTrigger)
            {
                MapSchemas(contentTrigger.Schemas, map);
                MapSchemas(contentTrigger.ReferencedSchemas, map);
            }
        }
    }

    private void MapSchemas(List<SchemaCondition>? schemas, Dictionary<string, string> schemaMap)
    {
        if (schemas == null)
        {
            return;
        }

        foreach (var schema in schemas)
        {
            if (!schemaMap.TryGetValue(schema.SchemaId!, out var found))
            {
                log.WriteLine($"Schema {schema.SchemaId} not found.");
            }

            schema.SchemaId = found;
        }
    }

    private static IEnumerable<IFile> GetFiles(IFileSystem fs)
    {
        foreach (var file in fs.GetFiles(new FilePath("rules"), ".json"))
        {
            if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
            {
                yield return file;
            }
        }
    }

    public async Task GenerateSchemaAsync(ISyncService sync)
    {
        await sync.WriteJsonSchemaAsync<RuleModel>(new FilePath("rule.json"));

        var sample = new RuleModel
        {
            Name = "My-Rule",
            Trigger = new ContentChangedRuleTriggerDto
            {
                HandleAll = true
            },
            TypedAction = new WebhookRuleActionDto
            {
                Url = new Uri("https://squidex.io")
            },
            IsEnabled = true
        };

        await sync.WriteWithSchema(new FilePath("rules", "__rule.json"), sample, Ref);
    }
}
