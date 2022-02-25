// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.Sync
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class InheritanceAttribute : Attribute
    {
        public string Discriminator { get; }

        public InheritanceAttribute(string discriminator)
        {
            Discriminator = discriminator;
        }
    }
}
