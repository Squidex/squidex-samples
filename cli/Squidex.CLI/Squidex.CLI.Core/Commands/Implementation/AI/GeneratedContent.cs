// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class GeneratedContent
{
    public List<UpsertSchemaFieldDto> SchemaFields { get; set; }

    public string SchemaName { get; set; }

    public List<Dictionary<string, object>> Contents { get; set; }
}
