// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NSwag;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace CodeGeneration
{
    public static class Program
    {
        public static void Main()
        {
            string appName = "XXX";

            // You can autogenerate classes from Squidex's cloud version
            string swaggerUrl = $"https://cloud.squidex.io/api/content/{appName}/swagger/v1/swagger.json";
            // Or your local Squidex instance
            // string swaggerUrl = "http://localhost:5000/api/swagger/v1/swagger.json";

            // Adjust this outputhPath to a file path in the project where you want to use Squidex
            string outputhPath = @"..\..\..\SquidexGenerated.cs";

            CodeGenerator.Generate(
                swaggerUrl: swaggerUrl,
                generatedNamespace: $"{appName}.SquidexModels",
                outputPath: outputhPath);
        }
    }
}
