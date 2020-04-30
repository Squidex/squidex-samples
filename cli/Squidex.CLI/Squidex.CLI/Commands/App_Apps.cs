// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using CommandDotNet;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "apps", Description = "Manages apps.")]
        [SubCommand]
        public sealed class Apps
        {
            private readonly IConfigurationService configuration;
            private readonly ILogger log;

            public Apps(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command(Name = "create", Description = "Creates a squidex app.")]
            public async Task Create(CreateArguments arguments)
            {
                var session = configuration.StartSession();

                var name = arguments.Name;

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = session.App;
                }

                var request = new CreateAppDto
                {
                    Name = name
                };

                await session.Apps.PostAppAsync(request);

                log.WriteLine("> App created.");
            }

            public sealed class CreateArguments : IArgumentModel
            {
                [Operand(Name = "name", Description = "The name of the app. If not provided then app configured in currentApp gets created.")]
                public string Name { get; set; }
            }
        }
    }
}
