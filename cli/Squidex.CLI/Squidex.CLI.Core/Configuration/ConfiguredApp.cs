// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Configuration;

public record ConfiguredApp
{
    public string ServiceUrl { get; init; }

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string Name { get; init; }

    public Dictionary<string, string>? Headers { get; init; }

    public bool IgnoreSelfSigned { get; init; }
}
