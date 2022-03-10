// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Models
{
    public static class SchemaWithRefsExtensions
    {
        public static SchemaWithRefs<SchemaDto> EnrichSchemaNames(this SchemaWithRefs<SchemaDto> target, ICollection<SchemaDto> allSchemas)
        {
            void Handle(ReferencesFieldPropertiesDto properties)
            {
                var referenced = allSchemas.FirstOrDefault(x => properties.SchemaIds?.Contains(x.Id) == true);

                if (referenced != null)
                {
                    target.ReferencedSchemas[referenced.Id] = referenced.Name;
                }
            }

            foreach (var field in target.Schema.Fields.OrEmpty())
            {
                if (field.Properties is ReferencesFieldPropertiesDto reference)
                {
                    Handle(reference);
                }

                foreach (var nested in field.Nested.OrEmpty())
                {
                    if (nested.Properties is ReferencesFieldPropertiesDto nestedReference)
                    {
                        Handle(nestedReference);
                    }
                }
            }

            return target;
        }

        public static SchemaWithRefs<T> AdjustReferences<T>(this SchemaWithRefs<T> target, ICollection<SchemaDto> allSchemas) where T : UpsertSchemaDto
        {
            void Handle(ReferencesFieldPropertiesDto properties)
            {
                if (properties.SchemaIds?.Count > 0)
                {
                    var newSchemaIds = properties.SchemaIds.ToList();

                    var i = 0;

                    foreach (var schemaId in properties.SchemaIds)
                    {
                        if (target.ReferencedSchemas.TryGetValue(schemaId, out var name))
                        {
                            var referenced = allSchemas.FirstOrDefault(x => x.Name == name);

                            if (referenced != null)
                            {
                                newSchemaIds[i] = referenced.Id;
                            }
                        }

                        i++;
                    }

                    properties.SchemaIds = newSchemaIds;
                }
            }

            foreach (var field in target.Schema.Fields.OrEmpty())
            {
                if (field.Properties is ReferencesFieldPropertiesDto reference)
                {
                    Handle(reference);
                }

                foreach (var nested in field.Nested.OrEmpty())
                {
                    if (nested.Properties is ReferencesFieldPropertiesDto nestedReference)
                    {
                        Handle(nestedReference);
                    }
                }
            }

            return target;
        }
    }
}
