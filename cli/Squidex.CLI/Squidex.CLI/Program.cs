// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using CommandDotNet;
using CommandDotNet.FluentValidation;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Squidex.CLI.Commands;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.Sync;
using Squidex.CLI.Commands.Implementation.Sync.App;
using Squidex.CLI.Commands.Implementation.Sync.Assets;
using Squidex.CLI.Commands.Implementation.Sync.Contents;
using Squidex.CLI.Commands.Implementation.Sync.Rules;
using Squidex.CLI.Commands.Implementation.Sync.Schemas;
using Squidex.CLI.Commands.Implementation.Sync.Workflows;
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
                    .AddSingleton<App>()
                    .AddSingleton<App.Apps>()
                    .AddSingleton<App.Backup>()
                    .AddSingleton<App.Config>()
                    .AddSingleton<App.Content>()
                    .AddSingleton<App.Log>()
                    .AddSingleton<App.Schemas>()
                    .AddSingleton<App.Sync>()
                    .AddSingleton<App.Twitter>()
                    .AddSingleton<ISynchronizer, AppSynchronizer>()
                    .AddSingleton<ISynchronizer, AssetsSynchronizer>()
                    .AddSingleton<ISynchronizer, ContentsSynchronizer>()
                    .AddSingleton<ISynchronizer, RulesSynchronizer>()
                    .AddSingleton<ISynchronizer, SchemasSynchronizer>()
                    .AddSingleton<ISynchronizer, WorkflowsSynchronizer>()
                    .AddSingleton<ILogger, ConsoleLogger>()
                    .AddSingleton<Synchronizer>()
                    .BuildServiceProvider();

            try
            {
                var appRunner =
                    new AppRunner<App>()
                        .UseMicrosoftDependencyInjection(serviceProvider)
                        .UseFluentValidation(true);

                return appRunner.Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex);
                return -1;
            }
        }
    }
}
