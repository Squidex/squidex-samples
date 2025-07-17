// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using Betalgo.Ranul.OpenAI;
using Betalgo.Ranul.OpenAI.Managers;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;
using Markdig;
using Markdig.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.CLI.Properties;

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class AIContentGenerator(IQueryCache queryCache)
{
    public async Task<GeneratedContent> GenerateAsync(GenerateRequest request,
        CancellationToken ct = default)
    {
        var cached = await queryCache.GetAsync(request.Description, ct);
        if (cached != null)
        {
            return cached;
        }

        var result = await GenerateCoreAsync(request, ct);

        await queryCache.StoreAsync(request.Description, result, ct);
        return result;
    }

    private static async Task<GeneratedContent> GenerateCoreAsync(GenerateRequest request,
        CancellationToken ct = default)
    {
        var client = new OpenAIService(new OpenAIOptions
        {
            ApiKey = request.OpenAIApiKey,
        });

        var systemPrompt = request.SystemPrompt;
        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            systemPrompt = Encoding.Default.GetString(Resources.AIPrompt);
        }

        var chatRequest = new ChatCompletionCreateRequest
        {
            Messages =
            [
                ChatMessage.FromSystem(systemPrompt),
                ChatMessage.FromSystem($"Language Codes: {string.Join(',', request.Languages)}"),
                ChatMessage.FromUser($"Create a schema for: {request.Description}"),
            ],
            Model = request.OpenAIChatModel,
        };

        if (request.NumberOfContentItems > 0)
        {
            chatRequest.Messages.Add(ChatMessage.FromUser($"Create furthermore {request.NumberOfContentItems} sample content items."));
        }

        for (var attempt = 1; attempt <= request.NumberOfAttempts; attempt++)
        {
            var chatResponse = await client.ChatCompletion.CreateCompletion(chatRequest, cancellationToken: ct);

            var (parsed, error) = ParseResult(request, chatResponse);
            if (parsed != null)
            {
                return parsed;
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                if (attempt == request.NumberOfAttempts)
                {
                    ThrowParsingException(error);
                    return null!;
                }

                chatRequest.Messages.Add(ChatMessage.FromUser(error));
            }
            else if (parsed == null)
            {
                break;
            }
        }

        ThrowParsingException("Unknown");
        return null!;
    }

    private static (GeneratedContent?, string? Error) ParseResult(GenerateRequest request, ChatCompletionCreateResponse response)
    {
        var error = response.Error;
        if (error != null)
        {
            throw new CLIException($"Failed to get response. {error.FormatError(response.HttpStatusCode)}");
        }

        var content = response.Choices.FirstOrDefault()?.Message.Content;
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new CLIException($"Failed to get image. No result provided.");
        }

        var parsed = Markdown.Parse(content);

        var codeBlocks = parsed.OfType<FencedCodeBlock>().ToList();
        if (codeBlocks.Count < 1 || codeBlocks.Count > 2)
        {
            return (null, "One or two code blocks expected");
        }

        var (schema, _) = ParseJson<SimplifiedSchema>(codeBlocks[0]);
        if (schema == null)
        {
            return (null, "Schema does not match to the provided sample");
        }

        var validator = new Validator(schema, request.Languages);

        var schemaErrors = validator.ValidateSchema();
        if (schemaErrors.Count > 0)
        {
            return (null, FormatError(schemaErrors, "Schema is not valid. Correct the following errors:"));
        }

        var result = new GeneratedContent { Schema = schema };
        if (request.NumberOfContentItems > 0 && codeBlocks.Count < 2)
        {
            return (null, $"Total number of request content items {request.NumberOfContentItems}, but no code block found.");
        }

        if (codeBlocks.Count >= 2)
        {
            var (contents, _) = ParseJson<List<Dictionary<string, JToken>>>(codeBlocks[1]);
            if (contents == null)
            {
                return (null, "Contents do not match to the provided sample");
            }

            if (request.NumberOfContentItems != contents.Count)
            {
                return (null, $"Total number of request content items {request.NumberOfContentItems}, got {contents.Count}");
            }

            var contentErrors = validator.ValidateContents(contents);
            if (contentErrors.Count > 0)
            {
                return (null, FormatError(contentErrors, "Contents are not valid. Correct the following errors:"));
            }

            result.Contents.AddRange(contents);
        }

        if (request.SchemaName != null)
        {
            result.Schema.Name = request.SchemaName;
        }

        return (result, null);
    }

    private static string FormatError(IReadOnlyList<string> errors, string startText)
    {
        var sb = new StringBuilder().AppendLine(startText);

        foreach (var error in errors)
        {
            sb.AppendLine($" * {error}");
        }

        return sb.ToString();
    }

    private static void ThrowParsingException(string reason)
    {
        throw new CLIException($"OpenAPI does not return a parsable result: {reason}.");
    }

    private static (T?, string?) ParseJson<T>(LeafBlock block) where T : class
    {
        try
        {
            var jsonText = GetText(block);

            return (JsonConvert.DeserializeObject<T>(jsonText), jsonText);
        }
        catch (JsonException)
        {
            return default;
        }

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
