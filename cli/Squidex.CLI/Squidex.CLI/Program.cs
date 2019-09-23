// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using FluentValidation.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Squidex.CLI.Commands;
using Squidex.CLI.Configuration;

namespace Squidex.CLI
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var serviceCollection =
                new ServiceCollection()
                    .AddSingleton<IConfigurationService, ConfigurationService>();

            foreach (var validator in GetAllValidatorTypes())
            {
                serviceCollection.AddSingleton(validator);
            }

            var serviceProvider = serviceCollection.BuildServiceProvider();

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

        private static IEnumerable<Type> GetAllValidatorTypes() =>
            typeof(Program)
                .Assembly
                .GetTypes()
                .SelectMany(t => t
                    .GetCustomAttributes(typeof(ValidatorAttribute), false)
                    .Select(a => ((ValidatorAttribute)a).ValidatorType));
    }
}
