// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public sealed class ExtendableRules : Resource
    {
        public List<CreateExtendableRule> Items { get; set; }
    }
}