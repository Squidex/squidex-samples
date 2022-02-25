// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;

namespace Squidex.CLI.Commands.Implementation.Sync.AssertFolders
{
    public sealed class AssetFoldersModel
    {
        [Required]
        public List<string> Paths { get; set; }
    }
}
