// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;

namespace Squidex.CLI.Commands.Implementation.Sync.Model
{
    [Inheritance("actionType")]
    public sealed class DynamicRuleAction : JObject
    {
        public DynamicRuleAction()
        {
        }

        public DynamicRuleAction(JObject other)
            : base(other)
        {
        }
    }
}
