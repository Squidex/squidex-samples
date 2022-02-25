// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentsModel
    {
        [Required]
        public List<ContentModel> Contents { get; set; }
    }
}
