﻿// ==========================================================================
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

namespace Squidex.CLI.Commands.Implementation.Sync.Model
{
    public sealed class RulesSynchronizer : ISynchronizer
    {
        private readonly ILogger log;

        public string Name => "Rules";

        public RulesSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public async Task ExportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var current = await session.Rules.GetRulesAsync();

            var index = 0;

            foreach (var rule in current.Items.OrderBy(x => x.Created))
            {
                var ruleName = rule.Name;

                if (string.IsNullOrWhiteSpace(ruleName))
                {
                    ruleName = "<Unnammed>";
                }

                await log.DoSafeAsync($"Exporting {ruleName} ({rule.Id})", async () =>
                {
                    await jsonHelper.WriteWithSchemaAs<RuleModel>(directoryInfo, $"rules/rule{index}.json", rule, "../__json/rule");
                });

                index++;
            }
        }

        public async Task ImportAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            var newRules = GetRuleFiles(directoryInfo, jsonHelper).ToList();

            if (!newRules.HasDistinctNames(x => x.Name))
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

            if (!options.NoDeletion)
            {
                foreach (var (name, rule) in rulesByName.ToList())
                {
                    if (!newRules.Any(x => x.Name == name))
                    {
                        await log.DoSafeAsync($"Rule '{name}' deleting", async () =>
                        {
                            await session.Rules.DeleteRuleAsync(rule.Id);

                            rulesByName.Remove(name);
                        });
                    }
                }
            }

            foreach (var newRule in newRules)
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

            foreach (var newRule in newRules)
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

        private IEnumerable<RuleModel> GetRuleFiles(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            foreach (var file in directoryInfo.GetFiles("rules\\*.json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    var rule = jsonHelper.Read<RuleModel>(file, log);

                    yield return rule;
                }
            }
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<RuleModel>(directoryInfo, "rule.json");

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

            await jsonHelper.WriteWithSchema(directoryInfo, "rules/__rule.json", sample, "../__json/rule");
        }
    }
}
