// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "config", Description = "Manage configurations.")]
        [SubCommand]
        public sealed class Config
        {
            private readonly IConfigurationService configuration;
            private readonly ILogger log;

            public Config(IConfigurationService configuration, ILogger log)
            {
                this.configuration = configuration;

                this.log = log;
            }

            [Command(Name = "list", Description = "Shows the current configuration.")]
            public void List(ListArguments arguments)
            {
                var config = configuration.GetConfiguration();

                if (arguments.Table)
                {
                    var table = new ConsoleTable("Name", "App", "ClientId", "ClientSecret", "Url");

                    foreach (var (key, app) in config.Apps)
                    {
                        table.AddRow(key, app.Name, app.ClientId, app.ClientSecret.Truncate(10), app.ServiceUrl);
                    }

                    table.Write(Format.Default);

                    log.WriteLine();
                    log.WriteLine("Current App: {0}", config.CurrentApp);
                }
                else
                {
                    log.WriteLine(config.JsonPrettyString());
                }
            }

            [Command(Name = "add", Description = "Add or update an app.")]
            public void Add(AddArguments arguments)
            {
                configuration.Upsert(arguments.ToEntryName(), arguments.ToModel());

                log.WriteLine("> App added.");
            }

            [Command(Name = "use", Description = "Use an app.")]
            public void Use(UseArguments arguments)
            {
                configuration.UseApp(arguments.Name);

                log.WriteLine("> App selected.");
            }

            [Command(Name = "remove", Description = "Remove an app.")]
            public void Remove(RemoveArguments arguments)
            {
                configuration.Remove(arguments.Name);

                log.WriteLine("> App removed.");
            }

            [Command(Name = "reset", Description = "Reset the config.")]
            public void Reset()
            {
                configuration.Reset();

                log.WriteLine("> Config reset.");
            }

            [Validator(typeof(Validator))]
            public sealed class ListArguments : IArgumentModel
            {
                [Option(LongName = "table", ShortName = "t", Description = "Output as table")]
                public bool Table { get; set; }

                public sealed class Validator : AbstractValidator<ListArguments>
                {
                }
            }

            [Validator(typeof(Validator))]
            public sealed class RemoveArguments : IArgumentModel
            {
                [Operand(Name = "name", Description = "The name of the app.")]
                public string Name { get; set; }

                public sealed class Validator : AbstractValidator<RemoveArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Name).NotEmpty();
                    }
                }
            }

            [Validator(typeof(Validator))]
            public sealed class UseArguments : IArgumentModel
            {
                [Operand(Name = "name", Description = "The name of the app.")]
                public string Name { get; set; }

                public sealed class Validator : AbstractValidator<UseArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Name).NotEmpty();
                    }
                }
            }

            [Validator(typeof(Validator))]
            public sealed class AddArguments : IArgumentModel
            {
                [Operand(Name = "name", Description = "The name of the app.")]
                public string Name { get; set; }

                [Operand(Name = "client-id", Description = "The client id.")]
                public string ClientId { get; set; }

                [Operand(Name = "client-secret", Description = "The client secret.")]
                public string ClientSecret { get; set; }

                [Option(LongName = "url", ShortName = "u", Description = "The optional url to your squidex installation. Default: https://cloud.squidex.io")]
                public string ServiceUrl { get; set; }

                [Option(LongName = "label", ShortName = "l", Description = "Optional label for this app.")]
                public string Label { get; set; }

                [Option(LongName = "create", ShortName = "c", Description = "Create the app if it does not exist (needs admin client)")]
                public bool Create { get; set; }

                public string ToEntryName()
                {
                    return !string.IsNullOrWhiteSpace(Label) ? Label : Name;
                }

                public ConfiguredApp ToModel()
                {
                    return new ConfiguredApp
                    {
                        Name = Name,
                        ClientId = ClientId,
                        ClientSecret = ClientSecret,
                        ServiceUrl = ServiceUrl
                    };
                }

                public sealed class Validator : AbstractValidator<AddArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.Name).NotEmpty();
                        RuleFor(x => x.ClientId).NotEmpty();
                        RuleFor(x => x.ClientSecret).NotEmpty();
                    }
                }
            }
        }
    }
}
