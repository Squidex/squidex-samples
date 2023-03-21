// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation;

public interface ISession
{
    string App { get; }

    string ClientId { get; }

    string ClientSecret { get; }

    string Url { get; }

    DirectoryInfo WorkingDirectory { get; }

    ISquidexClient Client { get; }
}
