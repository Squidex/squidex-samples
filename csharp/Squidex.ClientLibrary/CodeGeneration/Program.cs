// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace CodeGeneration;

public static partial class Program
{
    public static async Task Main()
    {
        var document = await OpenApiDocument.FromUrlAsync("https://localhost:5001/api/swagger/v1/swagger.json");

        SchemaCleaner.AddExtensions(document);

        // We write a more complete schema for fern code generation.
        File.WriteAllText(@"..\..\..\..\..\..\..\sdk-fern\fern\api\openapi\openapi.json", document.ToJson().UseCloudUrl());

        // This cleanup is only needed for .NET.
        SchemaCleaner.RemoveAppName(document);
        SchemaCleaner.RemoveUnusedSchemas(document);

        var generatorSettings = new CSharpClientGeneratorSettings();
        generatorSettings.CSharpGeneratorSettings.ArrayBaseType = "System.Collections.Generic.List";
        generatorSettings.CSharpGeneratorSettings.ArrayInstanceType = "System.Collections.Generic.List";
        generatorSettings.CSharpGeneratorSettings.ArrayType = "System.Collections.Generic.List";
        generatorSettings.CSharpGeneratorSettings.DictionaryBaseType = "System.Collections.Generic.Dictionary";
        generatorSettings.CSharpGeneratorSettings.DictionaryInstanceType = "System.Collections.Generic.Dictionary";
        generatorSettings.CSharpGeneratorSettings.DictionaryType = "System.Collections.Generic.Dictionary";
        generatorSettings.CSharpGeneratorSettings.ExcludedTypeNames = new[] { "JsonInheritanceConverter" };
        generatorSettings.CSharpGeneratorSettings.Namespace = "Squidex.ClientLibrary.Management";
        generatorSettings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = false;
        generatorSettings.CSharpGeneratorSettings.TemplateDirectory = Directory.GetCurrentDirectory();
        generatorSettings.CSharpGeneratorSettings.ValueGenerator = new ValueGenerator(generatorSettings.CSharpGeneratorSettings);
        generatorSettings.ExceptionClass = "SquidexManagementException";
        generatorSettings.GenerateClientInterfaces = true;
        generatorSettings.GenerateOptionalParameters = true;
        generatorSettings.OperationNameGenerator = new TagNameGenerator();
        generatorSettings.UseBaseUrl = false;

        var sourceCode =
            new CSharpClientGenerator(document, generatorSettings)
                .GenerateFile().UseFixedVersion(); // Use a static version to keep the changes low.

        File.WriteAllText(@"..\..\..\..\Squidex.ClientLibrary\Management\Generated.cs", sourceCode);
    }
}
