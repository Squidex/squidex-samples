// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Models;

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

    public static SchemaWithRefs<T> AdjustReferences<T>(this SchemaWithRefs<T> target,
        ICollection<SchemaDto> allSchemas) where T : UpsertSchemaDto
    {
        List<string>? Handle(List<string>? schemaIds)
        {
            if (schemaIds?.Count > 0)
            {
                var newSchemaIds = schemaIds.ToList();

                var i = 0;

                foreach (var schemaId in schemaIds)
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

                return newSchemaIds;
            }

            return null;
        }

        foreach (var field in target.Schema.Fields.OrEmpty())
        {
            switch (field.Properties)
            {
                case ReferencesFieldPropertiesDto reference:
                    reference.SchemaIds = Handle(reference.SchemaIds);
                    break;
                case ComponentsFieldPropertiesDto componentsReference:
                    componentsReference.SchemaIds = Handle(componentsReference.SchemaIds);
                    break;
                case ComponentFieldPropertiesDto componentReference:
                    componentReference.SchemaIds = Handle(componentReference.SchemaIds);
                    break;
            }

            foreach (var nested in field.Nested.OrEmpty())
            {
                switch (nested.Properties)
                {
                    case ReferencesFieldPropertiesDto reference:
                        reference.SchemaIds = Handle(reference.SchemaIds);
                        break;
                    case ComponentsFieldPropertiesDto componentsReference:
                        componentsReference.SchemaIds = Handle(componentsReference.SchemaIds);
                        break;
                    case ComponentFieldPropertiesDto componentReference:
                        componentReference.SchemaIds = Handle(componentReference.SchemaIds);
                        break;
                }
            }
        }

        return target;
    }
}
