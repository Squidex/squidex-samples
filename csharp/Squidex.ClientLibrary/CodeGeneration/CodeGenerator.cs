// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace CodeGeneration
{
    public class CodeGenerator
    {
        public static void Generate(
            string swaggerUrl,
            string generatedNamespace,
            string outputPath)
        {
            var document = OpenApiDocument.FromUrlAsync(swaggerUrl).Result;

            var generatorSettings = new CSharpClientGeneratorSettings();
            generatorSettings.CSharpGeneratorSettings.Namespace = generatedNamespace;
            generatorSettings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = false;
            generatorSettings.GenerateOptionalParameters = true;
            generatorSettings.GenerateClientInterfaces = true;
            generatorSettings.ExceptionClass = "SquidexManagementException";
            generatorSettings.OperationNameGenerator = new TagNameGenerator();
            generatorSettings.UseBaseUrl = false;

            var codeGenerator = new CSharpClientGenerator(document, generatorSettings);

            var code = codeGenerator.GenerateFile();

            code = code.Replace(" = new FieldPropertiesDto();", string.Empty);
            code = code.Replace(" = new RuleTriggerDto();", string.Empty);

            File.WriteAllText(outputPath, code);
        }
    }

    public sealed class TagNameGenerator : MultipleClientsFromOperationIdOperationNameGenerator
    {
        public override string GetClientName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            if (operation.Tags?.Count == 1)
            {
                return operation.Tags[0];
            }

            return base.GetClientName(document, path, httpMethod, operation);
        }
    }
}
