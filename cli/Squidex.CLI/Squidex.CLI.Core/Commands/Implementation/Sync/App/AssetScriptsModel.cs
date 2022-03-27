// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.Sync.App
{
    public sealed class AssetScriptsModel
    {
        public string? Create { get; set; }

        public string? Update { get; set; }

        public string? Annotate { get; set; }

        public string? Move { get; set; }

        public string? Delete { get; set; }
    }
}
