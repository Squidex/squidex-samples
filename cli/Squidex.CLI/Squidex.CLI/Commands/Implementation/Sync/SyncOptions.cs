// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public sealed class SyncOptions
    {
        public string[] Targets { get; set; }

        public string[] Languages { get; set; }

        public bool NoDeletion { get; set; }
    }
}
