// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public class AssetsModel
    {
        [Required]
        public List<AssetModel> Assets { get; set; }
    }
}
