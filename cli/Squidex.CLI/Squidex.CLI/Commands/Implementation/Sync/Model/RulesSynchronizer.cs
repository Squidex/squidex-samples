// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Model
{
    public sealed class RulesSynchronizer : ISynchronizer
    {
        public Task SynchronizeAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper, SyncOptions options, ISession session)
        {
            return Task.CompletedTask;
        }

        public async Task GenerateSchemaAsync(DirectoryInfo directoryInfo, JsonHelper jsonHelper)
        {
            await jsonHelper.WriteJsonSchemaAsync<RuleSettings>(directoryInfo, "rule.json");

            var sample = new RuleSettings
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

            await jsonHelper.WriteSampleAsync(directoryInfo, "rules/__rule.json", sample, "../__json/rule");
        }
    }
}
