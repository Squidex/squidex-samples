// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Rules
{
    public static class Extensions
    {
        public static UpdateExtendableRuleDto ToUpdate(this RuleModel model)
        {
            return new UpdateExtendableRuleDto { Action = model.Action, Trigger = model.Trigger, Name = model.Name };
        }

        public static CreateExtendableRuleDto ToCreate(this RuleModel model)
        {
            return new CreateExtendableRuleDto { Action = model.Action, Trigger = model.Trigger };
        }
    }
}
