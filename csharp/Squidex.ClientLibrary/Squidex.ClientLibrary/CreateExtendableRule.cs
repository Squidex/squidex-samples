// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary
{
    public sealed class CreateExtendableRule
    {
        public RuleTriggerDto Trigger { get; set; }

        public JObject Action { get; set; }
    }
}