// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ContentModel
    {
        [Required]
        public string Schema { get; set; }

        [Required]
        public object Data { get; set; }

        public object Query { get; set; }
    }
}
