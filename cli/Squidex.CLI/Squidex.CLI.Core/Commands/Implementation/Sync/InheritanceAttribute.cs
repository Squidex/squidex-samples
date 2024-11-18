// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.Sync;

[AttributeUsage(AttributeTargets.All)]
public sealed class InheritanceAttribute(string discriminator) : Attribute
{
    public string Discriminator { get; } = discriminator;
}
