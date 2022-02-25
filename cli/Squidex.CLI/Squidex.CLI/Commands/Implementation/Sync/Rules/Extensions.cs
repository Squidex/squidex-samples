// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Rules
{
    public static class Extensions
    {
        public static UpdateExtendableRuleDto ToUpdate(this RuleModel model)
        {
            return SimpleMapper.Map(model, new UpdateExtendableRuleDto());
        }

        public static CreateExtendableRuleDto ToCreate(this RuleModel model)
        {
            return SimpleMapper.Map(model, new CreateExtendableRuleDto());
        }
    }
}
