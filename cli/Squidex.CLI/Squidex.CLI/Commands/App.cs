// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using CommandDotNet.Attributes;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [ApplicationMetadata(Name = "info", Description = "Shows information about the CLI.")]
        public void Info()
        {
            var version = typeof(App).Assembly.GetName().Version;

            Console.WriteLine($"Squidex CLI v{version}, API Compatibility >= 3.X");
        }
    }
}
