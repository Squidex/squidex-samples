// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using Markdig;
using Markdig.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class AIContentGenerator
{
    private readonly IConfigurationStore configurationStore;

    public AIContentGenerator(IConfigurationStore configurationStore)
    {
        this.configurationStore = configurationStore;
    }

    public async Task<GeneratedContent> GenerateAsync(string description, string apiKey, string? schemaName = null,
        CancellationToken ct = default)
    {
        var cachedResponse = await MakeRequestAsync(description, apiKey, ct);

        return ParseResult(schemaName, cachedResponse);
    }

    private async Task<ChatCompletionCreateResponse> MakeRequestAsync(string description, string apiKey,
        CancellationToken ct)
    {
        var client = new OpenAIService(new OpenAiOptions
        {
            ApiKey = apiKey,
        });

        var cacheKey = $"openapi/query-cache/{description.ToSha256Base64()}";
        var (cachedResponse, _) = configurationStore.Get<ChatCompletionCreateResponse>(cacheKey);

        if (cachedResponse == null)
        {
            cachedResponse = await client.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("Create a list as json array. The list is described as followed:"),
                    ChatMessage.FromUser(description),
                    ChatMessage.FromSystem("Also create a JSON object with the field names of this list as keys and the json type as value (string)."),
                    ChatMessage.FromSystem("Also create a JSON array that only contains the name of the list above as valid slug."),
                },
                Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo
            }, cancellationToken: ct);

            configurationStore.Set(cacheKey, cachedResponse);
        }

        return cachedResponse;
    }

    private static GeneratedContent ParseResult(string? schemaName, ChatCompletionCreateResponse cachedResponse)
    {
        var parsed = Markdown.Parse(cachedResponse.Choices[0].Message.Content ?? string.Empty);

        var codeBlocks = parsed.OfType<FencedCodeBlock>().ToList();
        if (codeBlocks.Count != 3)
        {
            ThrowParsingException("3 code blocks expected");
            return default!;
        }

        var schemaObject = ParseJson<JObject>(codeBlocks[1], "Schema");
        var schemaFields = new List<UpsertSchemaFieldDto>();

        foreach (var (key, value) in schemaObject)
        {
            var fieldType = value?.ToString();

            switch (fieldType?.ToLowerInvariant())
            {
                case "string":
                case "text":
                    schemaFields.Add(new UpsertSchemaFieldDto
                    {
                        Name = key!,
                        Properties = new StringFieldPropertiesDto()
                    });
                    break;
                case "double":
                case "float":
                case "int":
                case "integer":
                case "long":
                case "number":
                case "real":
                    schemaFields.Add(new UpsertSchemaFieldDto
                    {
                        Name = key!,
                        Properties = new NumberFieldPropertiesDto()
                    });
                    break;
                case "bit":
                case "bool":
                case "boolean":
                    schemaFields.Add(new UpsertSchemaFieldDto
                    {
                        Name = key!,
                        Properties = new BooleanFieldPropertiesDto()
                    });
                    break;
                default:
                    ThrowParsingException($"Unexpected field type '{fieldType}' for field '{key}'");
                    return default!;
            }
        }

        var nameArray = ParseJson<JArray>(codeBlocks[2], "SchemaName");
        if (nameArray.Count != 1)
        {
            ThrowParsingException("'SchemaName' json has an unexpected structure.");
            return default!;
        }

        if (string.IsNullOrWhiteSpace(schemaName))
        {
            schemaName = nameArray[0].ToString();
        }

        var contentsBlock = ParseJson<JArray>(codeBlocks[0], "Contents");
        var contentsList = new List<Dictionary<string, object>>();

        foreach (var obj in contentsBlock.OfType<JObject>())
        {
            contentsList.Add(obj.OfType<JProperty>().ToDictionary(x => x.Name, x => (object)x.Value));
        }

        return new GeneratedContent
        {
            SchemaFields = schemaFields,
            SchemaName = schemaName,
            Contents = contentsList
        };
    }

    private static void ThrowParsingException(string reason)
    {
        throw new InvalidOperationException($"OpenAPI does not return a parsable result: {reason}.");
    }

    private static T ParseJson<T>(LeafBlock block, string name) where T : JToken
    {
        JToken jsonNode;
        try
        {
            var jsonText = GetText(block);

            jsonNode = JToken.Parse(jsonText);
        }
        catch (JsonException)
        {
            ThrowParsingException($"'{name}' code is not valid json.");
            return default!;
        }

        if (jsonNode is not T typed)
        {
            ThrowParsingException($"'{name}' json has an unexpected structure.");
            return default!;
        }

        return typed;

        static string GetText(LeafBlock block)
        {
            var sb = new StringBuilder();

            var lines = block.Lines.Lines;

            if (lines != null)
            {
                foreach (var line in lines)
                {
                    sb.AppendLine(line.Slice.ToString());
                }
            }

            return sb.ToString();
        }
    }
}
