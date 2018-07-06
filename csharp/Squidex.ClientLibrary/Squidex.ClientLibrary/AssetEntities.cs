// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    public class AssetEntities
    {
        public IList<Asset> Items { get; } = new List<Asset>();

        public long Total { get; set; }
    }
}
