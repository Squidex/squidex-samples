// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas
{
    public static class Extensions
    {
        public static CreateSchemaDto ToRequest(this SchemaCreateModel model)
        {
            var isSingleton = model.IsSingleton;

            var type = model.SchemaType;

            if (model.IsSingleton && type == SchemaType.Default)
            {
                type = SchemaType.Singleton;
            }

            return new CreateSchemaDto { Name = model.Name, IsSingleton = isSingleton, Type = type };
        }
    }
}
