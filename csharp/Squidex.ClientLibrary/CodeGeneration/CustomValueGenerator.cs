// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;

namespace CodeGeneration;

public sealed class CustomValueGenerator(CSharpGeneratorSettings settings) : CSharpValueGenerator(settings)
{
    public override string? GetDefaultValue(JsonSchema schema, bool allowsNull, string targetType, string? typeNameHint, bool useSchemaDefault, TypeResolverBase typeResolver)
    {
        if (!string.IsNullOrWhiteSpace(schema.ActualDiscriminator))
        {
            return string.Empty;
        }

        return base.GetDefaultValue(schema, allowsNull, targetType, typeNameHint, useSchemaDefault, typeResolver);
    }
}
