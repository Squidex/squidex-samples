// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using CommandDotNet;
using FluentValidation;
using FluentValidation.Attributes;
using Namotion.Reflection;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary.Management;
using static Squidex.CLI.Commands.App.Config;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "apps", Description = "Manages apps.")]
        [SubCommand]
        public sealed class Apps
        {
            private readonly ILogger log;
            private readonly IConfigurationService configuration;

            public Apps(ILogger log, IConfigurationService configuration)
            {
                this.log = log;
                this.configuration = configuration;
            }

            [Command(Name = "create", Description = "Creates a squidex app.")]
            public async void Create(CreateArguments arguments)
            {
                await EnsureAppExistsAsync(arguments.Name);

                log.WriteLine("App created.");
            }

            private async Task EnsureAppExistsAsync(string name)
            {
                var session = configuration.StartSession();

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = session.App;
                }

                await log.DoSafeAsync("Creating app", async () =>
                {
                    var request = new CreateAppDto
                    {
                        Name = name
                    };

                    await session.Apps.PostAppAsync(request);
                });
            }

            public sealed class CreateArguments : IArgumentModel
            {
                [Operand(Name = "name", Description = "The name of the app. If not provided then app configured in currentApp gets created.")]
                public string Name { get; set; }
            }
        }
    }
}
