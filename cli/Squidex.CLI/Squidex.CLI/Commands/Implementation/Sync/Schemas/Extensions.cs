// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Squidex.ClientLibrary.Management;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Squidex.CLI.Commands.Implementation.Sync.Schemas
{
    public static class Extensions
    {
        public static CreateSchemaDto ToCreate(this SchemaCreateModel model)
        {
            var isSingleton = model.IsSingleton;

            var type = model.SchemaType;

            if (model.IsSingleton && type == SchemaType.Default)
            {
                type = SchemaType.Singleton;
            }

            return new CreateSchemaDto { Name = model.Name, IsSingleton = isSingleton, Type = type };
        }

        public static void MapReferences(this SynchronizeSchemaDto schema, Dictionary<string, string> map)
        {
            if (schema.Fields != null)
            {
                foreach (var field in schema.Fields)
                {
                    MapReferences(field, map);
                }
            }
        }

        public static void MapReferences(this UpsertSchemaFieldDto field, Dictionary<string, string> map)
        {
            MapReferences(field.Properties, map);

            if (field.Nested != null)
            {
                foreach (var nested in field.Nested)
                {
                    MapReferences(nested.Properties, map);
                }
            }
        }

        private static void MapReferences(FieldPropertiesDto properties, Dictionary<string, string> map)
        {
            if (properties is ReferencesFieldPropertiesDto references)
            {
                references.SchemaIds = MapReferences(references.SchemaIds, map);
            }
            else if (properties is ComponentFieldPropertiesDto component)
            {
                component.SchemaIds = MapReferences(component.SchemaIds, map);
            }
            else if (properties is ComponentsFieldPropertiesDto components)
            {
                components.SchemaIds = MapReferences(components.SchemaIds, map);
            }
        }

        private static List<string>? MapReferences(List<string> ids, Dictionary<string, string> map)
        {
            if (ids == null || ids.Count == 0)
            {
                return ids;
            }

            var result = new List<string>();

            foreach (var id in ids)
            {
                if (map.TryGetValue(id, out var target))
                {
                    result.Add(target);
                }
            }

            return result;
        }
    }
}
