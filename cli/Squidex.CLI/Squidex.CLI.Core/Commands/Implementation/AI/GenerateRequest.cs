// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class GenerateRequest
{
    public string? SchemaName { get; set; }

    public string Description { get; set; }

    public string OpenAIApiKey { get; set; }

    public string OpenAIChatModel { get; set; } = "gpt-4-turbo";

    public string OpenAIImageModel { get; set; } = "dall-e2";

    public int NumberOfContentItems { get; set; } = 3;

    public int NumberOfAttempts { get; set; } = 3;

    public bool GenerateImages { get; set; }

    public bool DeleteSchema { get; internal set; }

    public bool NoSchema { get; internal set; }

    public bool NoContents { get; internal set; }

    public string? SystemPrompt { get; set; }

    public HashSet<string> Languages { get; set; } = ["en"];
}
