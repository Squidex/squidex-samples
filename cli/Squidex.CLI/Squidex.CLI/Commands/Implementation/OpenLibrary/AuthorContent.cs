// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.ClientLibrary;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands.Implementation.OpenLibrary;

public sealed class AuthorContent : Content<AuthorData>
{
}

public sealed class AuthorData
{
    [JsonConverter(typeof(InvariantConverter))]
    public string? Name { get; set; }

    [JsonConverter(typeof(InvariantConverter))]
    public string? PersonalName { get; set; }

    [JsonConverter(typeof(InvariantConverter))]
    public string? Bio { get; set; }

    [JsonConverter(typeof(InvariantConverter))]
    public string? Birthdate { get; set; }

    [JsonConverter(typeof(InvariantConverter))]
    public string? Wikipedia { get; set; }
}
