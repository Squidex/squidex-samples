// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Rules
{
    public sealed class RulesSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/rule";
        private readonly ILogger log;

        public string Name => "Rules";

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

        public async Task ExportAsync(IFileSystem fs, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var current = await session.Rules.GetRulesAsync();

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
                    await jsonHelper.WriteWithSchemaAs<RuleModel>(fs, new FilePath("rules", $"rule{i}.json"), rule, Ref);
                });
            });
        }

        public async Task ImportAsync(IFileSystem fs, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var models =
                GetFiles(fs)
                    .Select(x => jsonHelper.Read<RuleModel>(x, log))
                    .ToList();

            if (!models.HasDistinctNames(x => x.Name))
            {
                log.WriteLine("ERROR: Can only sync rules when all target rules have distinct names.");
                return;
            }

            var current = await session.Rules.GetRulesAsync();

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
                    if (models.All(x => x.Name != name))
                    {
                        await log.DoSafeAsync($"Rule '{name}' deleting", async () =>
                        {
                            await session.Rules.DeleteRuleAsync(rule.Id);

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

                    var created = await session.Rules.CreateRuleAsync(request);

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

                    rule = await session.Rules.UpdateRuleAsync(rule.Id, request);

                    return rule.Version;
                });

                if (newRule.IsEnabled != rule.IsEnabled)
                {
                    if (newRule.IsEnabled)
                    {
                        await log.DoVersionedAsync($"Rule '{newRule.Name}' enabling", rule.Version, async () =>
                        {
                            var result = await session.Rules.EnableRuleAsync(rule.Id);

                            return result.Version;
                        });
                    }
                    else
                    {
                        await log.DoVersionedAsync($"Rule '{newRule.Name}' disabling", rule.Version, async () =>
                        {
                            var result = await session.Rules.DisableRuleAsync(rule.Id);

                            return result.Version;
                        });
                    }
                }
            }
        }

        private async Task MapSchemaIdsToNamesAsync(ISession session, ExtendableRules current)
        {
            var schemas = await session.Schemas.GetSchemasAsync(session.App);

            var map = schemas.Items.ToDictionary(x => x.Id, x => x.Name);

            foreach (var rule in current.Items)
            {
                if (rule.Trigger is ContentChangedRuleTriggerDto contentTrigger)
                {
                    MapSchemas(contentTrigger, map);
                }
            }
        }

        private async Task MapSchemaNamesToIdsAsync(ISession session, List<RuleModel> models)
        {
            var schemas = await session.Schemas.GetSchemasAsync(session.App);

            var map = schemas.Items.ToDictionary(x => x.Name, x => x.Id);

            foreach (var newRule in models)
            {
                if (newRule.Trigger is ContentChangedRuleTriggerDto contentTrigger)
                {
                    MapSchemas(contentTrigger, map);
                }
            }
        }

        private void MapSchemas(ContentChangedRuleTriggerDto dto, Dictionary<string, string> schemaMap)
        {
            foreach (var schema in dto.Schemas)
            {
                if (!schemaMap.TryGetValue(schema.SchemaId, out var found))
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

        public async Task GenerateSchemaAsync(IFileSystem fs, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<RuleModel>(fs, new FilePath("rule.json"));

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

            await jsonHelper.WriteWithSchema(fs, new FilePath("rules", "__rule.json"), sample, Ref);
        }
    }
}
