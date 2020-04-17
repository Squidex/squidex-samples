// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using CommandDotNet;
using Squidex.CLI.Commands.Implementation;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        private readonly ILogger log;

        public App(ILogger log)
        {
            this.log = log;
        }

        [Command(Name = "info", Description = "Shows information about the CLI.")]
        public void Info()
        {
            var version = typeof(App).Assembly.GetName().Version;

            log.WriteLine($"Squidex CLI v{version}, API Compatibility >= 4.X");
        }
    }
}
