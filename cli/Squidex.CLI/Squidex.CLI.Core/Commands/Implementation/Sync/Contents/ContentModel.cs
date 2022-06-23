// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel.DataAnnotations;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    internal sealed class ContentModel
    {
        [Required]
        public string Schema { get; set; }

        [Required]
        public DynamicData Data { get; set; }

        public string? Id { get; set; }

        public string? Status { get; set; }
    }
}
