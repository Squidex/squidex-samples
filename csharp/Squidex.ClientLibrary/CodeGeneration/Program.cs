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
        var rootFolder = Environment.GetEnvironmentVariable("SDKS_ROOT_FOLDER")!;

        if (string.IsNullOrEmpty(rootFolder))
        {
            Console.WriteLine("Configure the SDK root folder with 'SDKS_ROOT_FOLDER' environment variable.");
            return;
        }

        var document = await OpenApiDocument.FromUrlAsync("https://localhost:5001/api/swagger/v1/swagger.json");

        WriteToFile(rootFolder, document, "sdk-spec/openapi.json");

        SchemaCleaner.AddExtensions(document);

        WriteToFile(rootFolder, document, "sdk-java/openapi.json");
        WriteToFile(rootFolder, document, "sdk-node/openapi.json");
        WriteToFile(rootFolder, document, "sdk-php/openapi.json");

        SchemaCleaner.RemoveUnusedSchemas(document);

        var sourceCode = GenerateCode(document);

        File.WriteAllText(Path.Combine(rootFolder, "samples/csharp/Squidex.ClientLibrary/Squidex.ClientLibrary/Generated.cs"), sourceCode);
    }

    private static void WriteToFile(string rootFolder, OpenApiDocument document, string path)
    {
        var targetFile = new FileInfo(Path.Combine(rootFolder, path));

        if (targetFile.Directory!.Exists)
        {
            File.WriteAllText(targetFile.FullName, document.ToJson().UseCloudUrl());
        }
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
        generatorSettings.CSharpGeneratorSettings.ExcludedTypeNames = ["JsonInheritanceConverter"];
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
