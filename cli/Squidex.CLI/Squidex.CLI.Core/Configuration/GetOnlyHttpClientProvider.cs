// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Configuration;

namespace Squidex.CLI.Configuration;

public sealed class GetOnlyHttpClientProvider(SquidexOptions options) : StaticHttpClientProvider(options)
{
    protected override HttpMessageHandler CreateMessageHandler(SquidexOptions options)
    {
        var baseHandler = base.CreateMessageHandler(options);

        return new GetOnlyHttpMessageHandler
        {
            InnerHandler = baseHandler
        };
    }
}
