// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas;

internal sealed class SchemaModel : SchemaCreateModel
{
    public SynchronizeSchemaDto Schema { get; set; }
}
