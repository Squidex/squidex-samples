// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using Squidex.CLI.Commands.Implementation;

namespace Squidex.CLI.Commands;

public partial class App
{
    private readonly ILogger log;

    public App(ILogger log)
    {
        this.log = log;
    }

    [Command("info", Description = "Shows information about the CLI.")]
    public void Info()
    {
        var version = typeof(App).Assembly.GetName().Version;

        log.WriteLine($"Squidex CLI v{version}, API Compatibility >= 4.X");
    }

    public abstract class AppArguments : IArgumentModel
    {
        [Option("app", Description = "The name of the app. If not provided then app configured in currentApp gets created.")]
        public string App { get; set; }
    }
}
