// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace CodeGeneration
{
    public static class Program
    {
        public static void Main()
        {
            var document = OpenApiYamlDocument.FromUrlAsync("https://raw.githubusercontent.com/SebastianStehle/ce-connector-api/main/schema.yaml").Result;

            var generatorSettings = new CSharpControllerGeneratorSettings();
            generatorSettings.CSharpGeneratorSettings.Namespace = "Integration.Localize.Controllers";
            generatorSettings.CSharpGeneratorSettings.JsonLibrary = CSharpJsonLibrary.SystemTextJson;
            generatorSettings.CSharpGeneratorSettings.GenerateNullableReferenceTypes = true;
            generatorSettings.GenerateOptionalParameters = true;
            generatorSettings.UseCancellationToken = true;
            generatorSettings.GenerateClientInterfaces = true;
            generatorSettings.ControllerStyle = CSharpControllerStyle.Abstract;

            var codeGenerator = new CSharpControllerGenerator(document, generatorSettings);

            var code = codeGenerator.GenerateFile();

            code = code.Replace("public partial class UniqueIdMetadata", "public partial class UniqueIdMetadata : Dictionary<string, string>");

            File.WriteAllText(@"..\..\..\..\Integration.Localize\Controllers\Generated.cs", code);
        }
    }
}
