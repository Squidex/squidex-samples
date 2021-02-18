// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Squidex.CLI.Commands.Implementation.Sync.Rules
{
    [Inheritance("actionType")]
    public sealed class DynamicRuleAction : Dictionary<string, JToken>
    {
        public DynamicRuleAction()
        {
        }

        public DynamicRuleAction(JObject other)
            : base(other)
        {
        }

        public static implicit operator JObject(DynamicRuleAction other)
        {
            var result = new JObject();

            foreach (var (key, value) in other)
            {
                result[key] = value;
            }

            return result;
        }
    }
}
