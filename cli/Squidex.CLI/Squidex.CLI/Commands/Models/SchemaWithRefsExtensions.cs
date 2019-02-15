// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Models
{
    public static class SchemaWithRefsExtensions
    {
        public static SchemaWithRefs<SchemaDetailsDto> EnrichSchemaNames(this SchemaWithRefs<SchemaDetailsDto> target, ICollection<SchemaDto> allSchemas)
        {
            if (target.Schema.Fields != null)
            {
                foreach (var field in target.Schema.Fields)
                {
                    if (field.Properties is ReferencesFieldPropertiesDto reference)
                    {
                        var referenced = allSchemas.FirstOrDefault(x => x.Id == reference.SchemaId);

                        if (referenced != null)
                        {
                            target.ReferencedSchemas[referenced.Id] = referenced.Name;
                        }
                    }

                    if (field.Nested != null)
                    {
                        foreach (var nested in field.Nested)
                        {
                            if (nested.Properties is ReferencesFieldPropertiesDto nestedReference)
                            {
                                var referenced = allSchemas.FirstOrDefault(x => x.Id == nestedReference.SchemaId);

                                if (referenced != null)
                                {
                                    target.ReferencedSchemas[referenced.Id] = referenced.Name;
                                }
                            }
                        }
                    }
                }
            }

            return target;
        }

        public static SchemaWithRefs<T> AdjustReferences<T>(this SchemaWithRefs<T> target, ICollection<SchemaDto> allSchemas) where T : UpsertDto
        {
            if (target.Schema.Fields != null)
            {
                foreach (var field in target.Schema.Fields)
                {
                    if (field.Properties is ReferencesFieldPropertiesDto reference)
                    {
                        if (target.ReferencedSchemas.TryGetValue(reference.SchemaId, out var name))
                        {
                            var referenced = allSchemas.FirstOrDefault(x => x.Name == name);

                            if (referenced != null)
                            {
                                reference.SchemaId = referenced.Id;
                            }
                        }
                    }

                    if (field.Nested != null)
                    {
                        foreach (var nested in field.Nested)
                        {
                            if (nested.Properties is ReferencesFieldPropertiesDto nestedReference)
                            {
                                if (target.ReferencedSchemas.TryGetValue(nestedReference.SchemaId, out var name))
                                {
                                    var referenced = allSchemas.FirstOrDefault(x => x.Name == name);

                                    if (referenced != null)
                                    {
                                        nestedReference.SchemaId = referenced.Id;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return target;
        }
    }
}
