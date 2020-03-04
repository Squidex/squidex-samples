﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using CommandDotNet;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "info", Description = "Shows information about the CLI.")]
        public void Info()
        {
            var version = typeof(App).Assembly.GetName().Version;

            Console.WriteLine($"Squidex CLI v{version}, API Compatibility >= 4.X");
        }
    }
}
