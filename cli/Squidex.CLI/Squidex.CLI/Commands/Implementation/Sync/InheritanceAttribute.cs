﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public sealed class InheritanceAttribute : Attribute
    {
        public string Discriminator { get; }

        public InheritanceAttribute(string discriminator)
        {
            Discriminator = discriminator;
        }
    }
}
