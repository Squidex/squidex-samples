// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using CommandDotNet;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Squidex.CLI.Commands;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;

namespace Squidex.CLI
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var serviceProvider =
                new ServiceCollection()
                    .AddSingleton<IConfigurationService, ConfigurationService>()
                    .AddSingleton<App.Backup>()
                    .AddSingleton<App.Config>()
                    .AddSingleton<App.Content>()
                    .AddSingleton<App.Schemas>()
                    .AddSingleton<App.Sync>()
                    .AddSingleton<App.Twitter>()
                    .AddSingleton<ILogger, ConsoleLogger>()
                    .BuildServiceProvider();

            try
            {
                var appRunner = new AppRunner<App>().UseMicrosoftDependencyInjection(serviceProvider);

                return appRunner.Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
                return -1;
            }
        }
    }
}
