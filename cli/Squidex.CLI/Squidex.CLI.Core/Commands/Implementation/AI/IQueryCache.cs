// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.AI;

public interface IQueryCache
{
    Task<GeneratedContent?> GetAsync(string prompt,
        CancellationToken ct = default);

    Task StoreAsync(string prompt, GeneratedContent content,
        CancellationToken ct);
}
