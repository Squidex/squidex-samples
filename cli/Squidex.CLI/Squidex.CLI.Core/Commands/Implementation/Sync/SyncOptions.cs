// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.Sync;

public sealed class SyncOptions
{
    public string[] Targets { get; set; }

    public string[] Languages { get; set; }

    public ContentAction ContentAction { get; set; }

    public bool Delete { get; set; }

    public bool Recreate { get; set; }

    public bool UpdateCurrentClient { get; set; }

    public DateTimeOffset LookbackDate { get; set; }
}
