// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Configuration;

namespace Squidex.CLI.Configuration;

public sealed class CLIHttpClientProvider(SquidexOptions options, bool emulate, Dictionary<string, string>? headers) : StaticHttpClientProvider(options)
{
    protected override HttpMessageHandler CreateMessageHandler(SquidexOptions options)
    {
        var handler = base.CreateMessageHandler(options);

        if (emulate)
        {
            var newHandler = new GetOnlyHttpMessageHandler
            {
                InnerHandler = handler
            };

            handler = newHandler;
        }

        if (headers?.Count > 0)
        {
            var newHandler = new CustomHeadersMessageHandler(headers)
            {
                InnerHandler = handler
            };

            handler = newHandler;
        }

        return handler;
    }
}
