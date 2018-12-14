// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.ComponentModel.DataAnnotations;
using CommandDotNet;
using CommandDotNet.Attributes;
using ConsoleTables;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Configuration;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [ApplicationMetadata(Name = "config", Description = "Manage configurations.")]
        [SubCommand]
        public class Config
        {
            [InjectProperty]
            public IConfigurationService Configuration { get; set; }

            [ApplicationMetadata(Name = "view", Description = "Shows the current configuration.")]
            public void View([Option] bool json)
            {
                var config = Configuration.GetConfiguration();

                if (json)
                {
                    Console.WriteLine(config.JsonPrettyString());
                }
                else
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
            }

            [ApplicationMetadata(Name = "add", Description = "Add or update an app.")]
            public void Add(AddArguments arguments)
            {
                Configuration.Upsert(arguments.ToEntryName(), arguments.ToModel());

                Console.WriteLine("> App added.");
            }

            [ApplicationMetadata(Name = "remove", Description = "Remove an app.")]
            public void Remove(AppArguments arguments)
            {
                Configuration.Remove(arguments.Name);

                Console.WriteLine("> App removed.");
            }

            [ApplicationMetadata(Name = "use", Description = "Use an app.")]
            public void Use(AppArguments arguments)
            {
                Configuration.UseApp(arguments.Name);

                Console.WriteLine("> App selected.");
            }

            [Validator(typeof(AppArgumentsValidator))]
            public sealed class AppArguments : IArgumentModel
            {
                [Argument(Name = "name", Description = "The name of the app.")]
                public string Name { get; set; }

                public sealed class AppArgumentsValidator : AbstractValidator<AppArguments>
                {
                    public AppArgumentsValidator()
                    {
                        RuleFor(x => x.Name).NotEmpty();
                    }
                }
            }

            [Validator(typeof(AddArgumentsValidator))]
            public sealed class AddArguments : IArgumentModel
            {
                [Argument(Name = "name", Description = "The name of the app.")]
                public string Name { get; set; }

                [Argument(Name = "client-id", Description = "The client id.")]
                public string ClientId { get; set; }

                [Argument(Name = "client-secret", Description = "The client secret.")]
                public string ClientSecret { get; set; }

                [Option(LongName = "url", Description = "The optional url to your squidex installation. Default: https://cloud.squidex.io")]
                public string ServiceUrl { get; set; }

                [Option(LongName = "label", Description = "Optional label for this app.")]
                public string Label { get; set; }

                public string ToEntryName()
                {
                    return !string.IsNullOrWhiteSpace(Label) ? Label : Name;
                }

                public ConfiguredApp ToModel()
                {
                    return new ConfiguredApp { ClientId = ClientId, ClientSecret = ClientSecret, Name = Name };
                }

                public sealed class AddArgumentsValidator : AbstractValidator<AddArguments>
                {
                    public AddArgumentsValidator()
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
