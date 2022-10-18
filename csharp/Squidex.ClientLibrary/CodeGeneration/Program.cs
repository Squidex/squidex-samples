// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

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
            generatorSettings.CSharpGeneratorSettings.ArrayType = "System.Collections.Generic.List";
            generatorSettings.CSharpGeneratorSettings.ArrayInstanceType = "System.Collections.Generic.List";
            generatorSettings.CSharpGeneratorSettings.ArrayBaseType = "System.Collections.Generic.List";
            generatorSettings.CSharpGeneratorSettings.DictionaryBaseType = "System.Collections.Generic.Dictionary";
            generatorSettings.CSharpGeneratorSettings.DictionaryInstanceType = "System.Collections.Generic.Dictionary";
            generatorSettings.CSharpGeneratorSettings.DictionaryType = "System.Collections.Generic.Dictionary";
            generatorSettings.CSharpGeneratorSettings.TemplateDirectory = Directory.GetCurrentDirectory();
            generatorSettings.GenerateOptionalParameters = true;
            generatorSettings.GenerateClientInterfaces = true;
            generatorSettings.ExceptionClass = "SquidexManagementException";
            generatorSettings.OperationNameGenerator = new TagNameGenerator();
            generatorSettings.UseBaseUrl = false;

            var codeGenerator = new CSharpClientGenerator(document, generatorSettings);

            var code = codeGenerator.GenerateFile();

            // Fix the creation of abstract classes.
            code = code.Replace(" = new FieldPropertiesDto();", string.Empty);
            code = code.Replace(" = new RuleTriggerDto();", string.Empty);
            code = code.Replace(" = new RuleAction();", string.Empty);

            // Fix the wrong initialization of nested collections.
            code = code.Replace("new System.Collections.Generic.Dictionary<string, System.Collections.ObjectModel.Collection<CallsUsagePerDateDto>>();", "new System.Collections.Generic.Dictionary<string, System.Collections.Generic.ICollection<CallsUsagePerDateDto>>();");

            // Fix the custom field names property.
            code = code.Replace("public FieldNames", "public System.Collections.Generic.ICollection<string>");

            // Be more tolerant with response cpdes.
            code = code.Replace("if (status_ == 201)", "if (status_ == 201 || status_ == 200)");
            code = code.Replace("if (status_ == 200)", "if (status_ == 201 || status_ == 200)");

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
