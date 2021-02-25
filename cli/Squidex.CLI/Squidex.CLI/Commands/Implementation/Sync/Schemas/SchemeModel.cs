// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas
{
    public sealed class SchemeModel
    {
        public string Name { get; set; }

        public bool IsSingleton { get; set; }

        public bool IsPublished { get; set; }

        public SynchronizeSchemaDto Schema { get; set; }
    }
}
