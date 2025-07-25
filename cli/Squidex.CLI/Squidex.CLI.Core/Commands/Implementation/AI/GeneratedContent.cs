﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class GeneratedContent
{
    public SimplifiedSchema Schema { get; set; }

    public List<Dictionary<string, JToken>> Contents { get; } = [];
}

public record ReplaceableImage(SimplifiedImage Image, Action<JToken> Setter);
