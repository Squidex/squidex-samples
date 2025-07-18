﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.Sync;

public enum ContentAction
{
    Upsert,
    UpsertPatch,
    Create,
    Update,
    Patch,
}
