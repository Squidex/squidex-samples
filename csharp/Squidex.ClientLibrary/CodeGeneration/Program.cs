// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace CodeGeneration;

public static class Program
{
    public static async Task Main()
    {
        var document = await OpenApiDocument.FromUrlAsync("https://localhost:5001/api/swagger/v1/swagger.json");

        var rootFolder = Environment.GetEnvironmentVariable("SDKS_ROOT_FOLDER")!;

        SchemaCleaner.AddExtensions(document);

        // We write a more complete schema for fern code generation.
        File.WriteAllText(Path.Combine(rootFolder, "sdk-php/openapi.json"), document.ToJson().UseCloudUrl());
        File.WriteAllText(Path.Combine(rootFolder, "sdk-fern/fern/openapi/openapi.json"), document.ToJson().UseCloudUrl());
        File.WriteAllText(Path.Combine(rootFolder, "sdk-fern/other-generators/openapi.json"), document.ToJson().UseCloudUrl());

        // This cleanup is only needed for .NET.
        SchemaCleaner.RemoveAppName(document);

        // We also need a version without app name.
        File.WriteAllText(Path.Combine(rootFolder, "sdk-fern/other-generators/openapi-noapp.json"), document.ToJson().UseCloudUrl());

        SchemaCleaner.RemoveUnusedSchemas(document);

        var sourceCode = GenerateCode(document);

        File.WriteAllText(Path.Combine(rootFolder, "samples/csharp/Squidex.ClientLibrary/Squidex.ClientLibrary/Generated.cs"), sourceCode);
    }

    private static string GenerateCode(OpenApiDocument document)
    {
        var generatorSettings = new CSharpClientGeneratorSettings();
        generatorSettings.CSharpGeneratorSettings.TemplateDirectory = Directory.GetCurrentDirectory();
        generatorSettings.CSharpGeneratorSettings.ArrayBaseType = "System.Collections.Generic.List";
        generatorSettings.CSharpGeneratorSettings.ArrayInstanceType = "System.Collections.Generic.List";
        generatorSettings.CSharpGeneratorSettings.ArrayType = "System.Collections.Generic.List";
        generatorSettings.CSharpGeneratorSettings.DictionaryBaseType = "System.Collections.Generic.Dictionary";
        generatorSettings.CSharpGeneratorSettings.DictionaryInstanceType = "System.Collections.Generic.Dictionary";
        generatorSettings.CSharpGeneratorSettings.DictionaryType = "System.Collections.Generic.Dictionary";
        generatorSettings.CSharpGeneratorSettings.ExcludedTypeNames = new[] { "JsonInheritanceConverter" };
        generatorSettings.CSharpGeneratorSettings.Namespace = "Squidex.ClientLibrary";
        generatorSettings.CSharpGeneratorSettings.PropertyNameGenerator = new CustomPropertyNameGenerator();
        generatorSettings.CSharpGeneratorSettings.PropertySetterAccessModifier = string.Empty;
        generatorSettings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = false;
        generatorSettings.CSharpGeneratorSettings.ValueGenerator = new CustomValueGenerator(generatorSettings.CSharpGeneratorSettings);
        generatorSettings.ExceptionClass = "SquidexException";
        generatorSettings.GenerateClientInterfaces = true;
        generatorSettings.GenerateOptionalParameters = true;
        generatorSettings.InjectHttpClient = false;
        generatorSettings.OperationNameGenerator = new CustomTagNameGenerator();
        generatorSettings.UseBaseUrl = false;

        var sourceCode =
            new CSharpClientGenerator(document, generatorSettings)
                .GenerateFile().UseFixedVersion(); // Use a static version to keep the changes low.

        return sourceCode;
    }
}
