// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NSwag;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace CodeGeneration;

public static partial class Program
{
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
