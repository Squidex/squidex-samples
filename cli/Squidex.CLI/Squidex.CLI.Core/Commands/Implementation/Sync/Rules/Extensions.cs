// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Rules
{
    internal static class Extensions
    {
        public static UpdateExtendableRuleDto ToUpdate(this RuleModel model)
        {
            var result = SimpleMapper.Map(model, new UpdateExtendableRuleDto());

            result.Action = model.Action;

            return result;
        }

        public static CreateExtendableRuleDto ToCreate(this RuleModel model)
        {
            var result = SimpleMapper.Map(model, new CreateExtendableRuleDto());

            result.Action = model.Action;

            return result;
        }

        public static string TypeName(this RuleTriggerDto trigger)
        {
            const string Suffix = "RuleTriggerDto";

            var name = trigger.GetType().Name;

            if (name.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^Suffix.Length];
            }

            return name;
        }

        public static string TypeName(this DynamicRuleAction action)
        {
            return action.GetValueOrDefault("actionType")?.ToString() ?? string.Empty;
        }
    }
}
