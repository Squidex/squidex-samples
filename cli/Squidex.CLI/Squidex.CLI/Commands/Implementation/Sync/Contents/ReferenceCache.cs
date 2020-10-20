﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Squidex.CLI.Commands.Implementation.Sync.Contents
{
    public sealed class ReferenceCache : Dictionary<(string Schema, string Filter), string>
    {
    }
}
