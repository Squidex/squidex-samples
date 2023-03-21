// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Google.Protobuf.WellKnownTypes;
using Squidex.ClientLibrary;
using Squidex.ClientLibrary.Configuration;
using Squidex.ClientLibrary.Utils;

namespace Squidex.CLI.Configuration;

public sealed class GetOnlyHttpClientProvider : StaticHttpClientProvider
{
    public GetOnlyHttpClientProvider(SquidexOptions options)
        : base(options)
    {
    }

    protected override HttpMessageHandler CreateMessageHandler(SquidexOptions options)
    {
        var baseHandler = base.CreateMessageHandler(options);

        return new GetOnlyHttpMessageHandler
        {
            InnerHandler = baseHandler
        };
    }
}
