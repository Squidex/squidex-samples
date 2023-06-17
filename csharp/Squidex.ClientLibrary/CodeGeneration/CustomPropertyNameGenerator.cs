// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace CodeGeneration;

public sealed class CustomPropertyNameGenerator : CSharpPropertyNameGenerator
{
    public override string Generate(JsonSchemaProperty property)
    {
        if (property.Name == "_links")
        {
            return "Links";
        }

        return base.Generate(property);
    }
}
