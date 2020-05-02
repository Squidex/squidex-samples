// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NJsonSchema.Generation;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public sealed class GuidFixProcessor : ISchemaProcessor
    {
        public void Process(SchemaProcessorContext context)
        {
            foreach (var (_, property) in context.Schema.Properties)
            {
                if (property.Format == "guid")
                {
                    property.Format = null;
                }

                if (property.Item != null && property.Item.Format == "guid")
                {
                    property.Item.Format = null;
                }
            }
        }
    }
}
