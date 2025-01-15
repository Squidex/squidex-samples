// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public partial class App
{
    [Command("config", Description = "Manage configurations.")]
    [Subcommand]
    public sealed class Config(IConfigurationService configuration, ILogger log)
    {
        [Command("list", Description = "Shows the current configuration.")]
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

                log.WriteLine(table.ToString());
                log.WriteLine();
                log.WriteLine("Current App: {0}", config.CurrentApp);
            }
            else
            {
                log.WriteLine(config.JsonPrettyString());
            }
        }

        [Command("add", Description = "Add or update an app.")]
        public void Add(AddArguments arguments)
        {
            var entry = !string.IsNullOrWhiteSpace(arguments.Label) ? 
                arguments.Label :
                arguments.App;

            var headers = new Dictionary<string, string>();

            if (arguments.Header != null)
            {
                foreach (var header in arguments.Header)
                {
                    var parts = header.Split('=');
                    if (parts.Length == 1)
                    {
                        continue;
                    }

                    var key = parts[0].Trim();
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    var value = string.Join('=', parts.Skip(1)).Trim();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }

                    headers[key] = value;
                }
            }

            var appConfig = new ConfiguredApp
            {
                Name = arguments.App,
                ClientId = arguments.ClientId,
                ClientSecret = arguments.ClientSecret,
                IgnoreSelfSigned = arguments.IgnoreSelfSigned,
                Headers = headers,
                ServiceUrl = arguments.ServiceUrl
            };

            configuration.Upsert(entry, appConfig);

            if (arguments.Use)
            {
                configuration.UseApp(arguments.App);

                log.Completed("App added and selected.");
            }
            else
            {
                log.Completed("App added.");
            }
        }

        [Command("use", Description = "Use an app.")]
        public void Use(UseArguments arguments)
        {
            configuration.UseApp(arguments.App);

            log.Completed("App selected.");
        }

        [Command("remove", Description = "Remove an app.")]
        public void Remove(RemoveArguments arguments)
        {
            configuration.Remove(arguments.App);

            log.Completed("App removed.");
        }

        [Command("reset", Description = "Reset the config.")]
        public void Reset()
        {
            configuration.Reset();

            log.Completed("Config reset.");
        }

        public sealed class ListArguments : IArgumentModel
        {
            [Option('t', "table", Description = "Output as table.")]
            public bool Table { get; set; }

            public sealed class Validator : AbstractValidator<ListArguments>
            {
            }
        }

        public sealed class RemoveArguments : IArgumentModel
        {
            [Operand("app", Description = "The name of the app.")]
            public string App { get; set; }

            public sealed class Validator : AbstractValidator<RemoveArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.App).NotEmpty();
                }
            }
        }

        public sealed class UseArguments : IArgumentModel
        {
            [Operand("app", Description = "The name of the app.")]
            public string App { get; set; }

            public sealed class Validator : AbstractValidator<UseArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.App).NotEmpty();
                }
            }
        }

        public sealed class AddArguments : IArgumentModel
        {
            [Operand("app", Description = "The name of the app.")]
            public string App { get; set; }

            [Operand("client-id", Description = "The client id.")]
            public string ClientId { get; set; }

            [Operand("client-secret", Description = "The client secret.")]
            public string ClientSecret { get; set; }

            [Option('u', "url", Description = "The optional url to your squidex installation. Default: https://cloud.squidex.io.")]
            public string ServiceUrl { get; set; }

            [Option('l', "label", Description = "Optional label for this app.")]
            public string Label { get; set; }

            [Option('c', "create", Description = "Create the app if it does not exist (needs admin client).")]
            public bool Create { get; set; }

            [Option('i', "ignore-self-signed", Description = "Ignores self signed certificates.")]
            public bool IgnoreSelfSigned { get; set; }

            [Option("use", Description = "Use the config.")]
            public bool Use { get; set; }

            [Option("header", Description = "Adds a custom header in the format (Key=Value)")]
            public string[] Header { get; set; }

            public sealed class Validator : AbstractValidator<AddArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.App).NotEmpty();
                    RuleFor(x => x.ClientId).NotEmpty();
                    RuleFor(x => x.ClientSecret).NotEmpty();
                }
            }
        }
    }
}
