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
    public static class Program
    {
        public static void Main()
        {
            var document = OpenApiDocument.FromUrlAsync("https://localhost:5001/api/swagger/v1/swagger.json").Result;

            var generatorSettings = new CSharpClientGeneratorSettings();
            generatorSettings.CSharpGeneratorSettings.Namespace = "Squidex.ClientLibrary.Management";
            generatorSettings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = false;
            generatorSettings.CSharpGeneratorSettings.ExcludedTypeNames = new[] { "JsonInheritanceConverter" };
            generatorSettings.GenerateOptionalParameters = true;
            generatorSettings.GenerateClientInterfaces = true;
            generatorSettings.ExceptionClass = "SquidexManagementException";
            generatorSettings.OperationNameGenerator = new TagNameGenerator();
            generatorSettings.UseBaseUrl = false;

            var codeGenerator = new CSharpClientGenerator(document, generatorSettings);

            var code = codeGenerator.GenerateFile();

            code = code.Replace(" = new FieldPropertiesDto();", string.Empty);
            code = code.Replace(" = new RuleTriggerDto();", string.Empty);
            code = code.Replace("new System.Collections.Generic.Dictionary<string, System.Collections.ObjectModel.Collection<CallsUsagePerDateDto>>();", "new System.Collections.Generic.Dictionary<string, System.Collections.Generic.ICollection<CallsUsagePerDateDto>>();");

            File.WriteAllText(@"..\..\..\..\Squidex.ClientLibrary\Management\Generated.cs", code);
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
}
