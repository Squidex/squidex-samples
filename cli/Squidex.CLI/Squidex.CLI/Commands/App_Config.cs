// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Configuration;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [Command(Name = "config", Description = "Manage configurations.")]
        [SubCommand]
        public sealed class Config
        {
            [InjectProperty]
            public IConfigurationService Configuration { get; set; }

            [Command(Name = "list", Description = "Shows the current configuration.")]
            public void List(ListArguments arguments)
            {
                var config = Configuration.GetConfiguration();

                if (arguments.Table)
                {
                    var table = new ConsoleTable("Name", "App", "ClientId", "ClientSecret", "Url");

                    foreach (var (key, app) in config.Apps)
                    {
                        table.AddRow(key, app.Name, app.ClientId, app.ClientSecret.Truncate(10), app.ServiceUrl);
                    }

                    table.Write(Format.Default);

                    Console.WriteLine();
                    Console.WriteLine("Current App: {0}", config.CurrentApp);
                }
                else
                {
                    Console.WriteLine(config.JsonPrettyString());
                }
            }

            [Command(Name = "add", Description = "Add or update an app.")]
            public void Add(AddArguments arguments)
            {
                Configuration.Upsert(arguments.ToEntryName(), arguments.ToModel());

                Console.WriteLine("> App added.");
            }

            [Command(Name = "remove", Description = "Remove an app.")]
            public void Remove(RemoveArguments arguments)
            {
                Configuration.Remove(arguments.Name);

                Console.WriteLine("> App removed.");
            }

            [Command(Name = "reset", Description = "Reset the config.")]
            public void Reset()
            {
                Configuration.Reset();

                Console.WriteLine("> Config reset.");
            }

            [Command(Name = "use", Description = "Use an app.")]
            public void Use(UseArguments arguments)
            {
                Configuration.UseApp(arguments.Name);

                Console.WriteLine("> App selected.");
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
                [Argument(Name = "name", Description = "The name of the app.")]
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
                [Argument(Name = "name", Description = "The name of the app.")]
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
                [Argument(Name = "name", Description = "The name of the app.")]
                public string Name { get; set; }

                [Argument(Name = "client-id", Description = "The client id.")]
                public string ClientId { get; set; }

                [Argument(Name = "client-secret", Description = "The client secret.")]
                public string ClientSecret { get; set; }

                [Option(LongName = "url", ShortName = "u", Description = "The optional url to your squidex installation. Default: https://cloud.squidex.io")]
                public string ServiceUrl { get; set; }

                [Option(LongName = "label", ShortName = "l", Description = "Optional label for this app.")]
                public string Label { get; set; }

                public string ToEntryName()
                {
                    return !string.IsNullOrWhiteSpace(Label) ? Label : Name;
                }

                public ConfiguredApp ToModel()
                {
                    return new ConfiguredApp { ClientId = ClientId, ClientSecret = ClientSecret, Name = Name, ServiceUrl = ServiceUrl };
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
