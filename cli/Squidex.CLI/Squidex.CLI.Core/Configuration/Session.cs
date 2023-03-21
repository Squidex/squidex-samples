// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Configuration;

public sealed class Session : ISession
{
    public DirectoryInfo WorkingDirectory { get; }

    public ISquidexClient Client { get; }

    public string App => Client.Options.AppName;

    public string ClientId => Client.Options.ClientId;

    public string ClientSecret => Client.Options.ClientSecret;

    public string Url => Client.Options.Url;

    public Session(DirectoryInfo workingDirectory, ISquidexClient client)
    {
        WorkingDirectory = workingDirectory;

        Client = client;
    }
}
