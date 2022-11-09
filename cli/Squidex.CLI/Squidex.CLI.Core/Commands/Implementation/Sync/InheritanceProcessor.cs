// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Reflection;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync;

public sealed class InheritanceProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var discriminator = GetDiscriminator(context.ContextualType);

        if (discriminator != null)
        {
            context.Schema.AllowAdditionalProperties = true;

            context.Schema.Properties[discriminator] = new JsonSchemaProperty
            {
                Type = JsonObjectType.String,
                IsRequired = true
            };
        }
    }

    private static string? GetDiscriminator(Type type)
    {
        var attribute = type.GetCustomAttribute<JsonConverterAttribute>();

        if (attribute != null &&
            attribute.ConverterType == typeof(JsonInheritanceConverter) &&
            attribute.ConverterParameters != null &&
            attribute.ConverterParameters.Length == 1)
        {
            return attribute.ConverterParameters[0] as string;
        }

        return type.GetCustomAttribute<InheritanceAttribute>()?.Discriminator;
    }
}
