// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas
{
    public static class Extensions
    {
        public static CreateSchemaDto ToRequest(this SchemaCreateModel model)
        {
            return new CreateSchemaDto { Name = model.Name, IsSingleton = model.IsSingleton };
        }
    }
}
