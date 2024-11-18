// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Squidex.ClientLibrary.ServiceExtensions;

internal sealed class SquidexClientProvider(IOptionsMonitor<SquidexServiceOptions> optionsMonitor) : ISquidexClientProvider
{
    private readonly ConcurrentDictionary<string, ISquidexClient> clients = new ConcurrentDictionary<string, ISquidexClient>();

    public ISquidexClient Get()
    {
        return Get(Options.DefaultName);
    }

    public ISquidexClient Get(string name)
    {
        return clients.GetOrAdd(name, (name, optionsMonitor) =>
        {
            var options = optionsMonitor.Get(name);

            return new SquidexClient(options);
        }, optionsMonitor);
    }
}
