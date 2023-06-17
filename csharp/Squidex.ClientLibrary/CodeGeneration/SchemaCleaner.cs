// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NJsonSchema;
using NSwag;
using Squidex.Text;

namespace CodeGeneration;

internal static class SchemaCleaner
{
    public static void AddExtensions(OpenApiDocument document)
    {
        static void AddExtensions(OpenApiOperation operation)
        {
            operation.ExtensionData ??= new Dictionary<string, object>();
            operation.ExtensionData["x-fern-sdk-group-name"] = operation.Tags[0].ToCamelCase();
            operation.ExtensionData["x-fern-sdk-method-name"] = operation.OperationId.Split('_').Last().ToCamelCase();

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Kind == OpenApiParameterKind.Path && parameter.Name == "app")
                {
                    parameter.ExtensionData ??= new Dictionary<string, object>();
                    parameter.ExtensionData["x-fern-sdk-variable"] = "appName";
                }
            }
        }

        foreach (var description in document.Operations)
        {
            AddExtensions(description.Operation);
        }

        document.ExtensionData ??= new Dictionary<string, object>();
        document.ExtensionData["x-fern-sdk-variables"] = new
        {
            appName = new
            {
                type = "string"
            }
        };
    }

    public static void RemoveAppName(OpenApiDocument document)
    {
        foreach (var description in document.Operations.ToList())
        {
            var parameters = description.Operation.Parameters;

            foreach (var parameter in parameters.ToList())
            {
                if (parameter.Kind == OpenApiParameterKind.Path && parameter.Name == "app")
                {
                    parameters.Remove(parameter);
                }
            }
        }
    }

    public static void RemoveUnusedSchemas(OpenApiDocument document)
    {
        var usedRefs = new Dictionary<JsonSchema, int>();

        void AddSchema(JsonSchema schema)
        {
            HandleSchema(schema, schema =>
            {
                usedRefs[schema] = usedRefs.GetValueOrDefault(schema) + 1;
            });
        }

        void RemoveSchema(JsonSchema schema)
        {
            HandleSchema(schema, schema =>
            {
                usedRefs[schema] = usedRefs.GetValueOrDefault(schema) - 1;

                if (usedRefs[schema] <= 0)
                {
                    var key = document.Definitions.FirstOrDefault(x => x.Value == schema).Key;

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        document.Definitions.Remove(key);
                    }
                }
            });
        }

        foreach (var description in document.Operations.ToList())
        {
            HandleOperation(description.Operation, AddSchema);
        }

        foreach (var (path, item) in document.Paths.ToList())
        {
            if (path.StartsWith("/api/content", StringComparison.OrdinalIgnoreCase))
            {
                document.Paths.Remove(path);

                foreach (var operation in item.Values)
                {
                    HandleOperation(operation, RemoveSchema);
                }
            }
        }
    }

    private static void HandleOperation(OpenApiOperation operation, Action<JsonSchema> handler)
    {
        foreach (var parameter in operation.Parameters)
        {
            handler(parameter);
            handler(parameter.Schema);
        }

        foreach (var response in operation.Responses)
        {
            handler(response.Value.Schema);
        }
    }

    private static void HandleSchema(JsonSchema schema, Action<JsonSchema> handler)
    {
        if (schema == null)
        {
            return;
        }

        handler(schema);

        if (schema.Item != null)
        {
            HandleSchema(schema.Item, handler);
        }

        if (schema.Reference != null)
        {
            HandleSchema(schema.Reference, handler);
        }

        foreach (var oneOf in schema.OneOf)
        {
            HandleSchema(oneOf, handler);
        }

        foreach (var allOf in schema.AllOf)
        {
            HandleSchema(allOf, handler);
        }

        foreach (var property in schema.Properties)
        {
            HandleSchema(property.Value, handler);
        }
    }
}
