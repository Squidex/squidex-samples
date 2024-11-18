// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Configuration;

public sealed class Session(DirectoryInfo workingDirectory, ISquidexClient client) : ISession
{
    public DirectoryInfo WorkingDirectory { get; } = workingDirectory;

    public ISquidexClient Client { get; } = client;

    public string App => Client.Options.AppName;

    public string ClientId => Client.Options.ClientId;

    public string ClientSecret => Client.Options.ClientSecret;

    public string Url => Client.Options.Url;
}
