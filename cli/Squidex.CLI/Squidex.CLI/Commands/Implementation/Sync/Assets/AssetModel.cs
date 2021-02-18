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
    public sealed class AssetModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string MimeType { get; set; }

        [Required]
        public string FileName { get; set; }

        public string FileHash { get; set; }

        public string Path { get; set; }

        public string Slug { get; set; }

        public ICollection<string> Tags { get; set; }

        public IDictionary<string, object> Metadata { get; set; }

        public bool IsProtected { get; set; }
    }
}
