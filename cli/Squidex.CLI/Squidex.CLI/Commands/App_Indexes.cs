// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using System.Text.RegularExpressions;
using CommandDotNet;
using ConsoleTables;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands;

#pragma warning disable MA0048 // File name must match type name

public sealed partial class App
{
    [Command("indexes", Description = "Manage indexes.")]
    [Subcommand]
    public sealed partial class Indexes(IConfigurationService configuration, ILogger log)
    {
        [Command("list", Description = "List all indexes.")]
        public async Task List(ListArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var indexes = await session.Client.Schemas.GetIndexesAsync(arguments.Schema);

            if (arguments.Table)
            {
                var table = new ConsoleTable("Name", "Fields");

                foreach (var index in indexes.Items)
                {
                    var fields = new StringBuilder();

                    foreach (var field in index.Fields)
                    {
                        fields.AppendLine($"{field.Name}: {field.Order}");
                    }

                    table.AddRow(index.Name, fields.ToString());
                }

                log.WriteLine(table.ToString());
            }
            else
            {
                log.WriteLine(indexes.JsonPrettyString());
            }
        }

        [Command("create", Description = "Create a new index.")]
        public async Task Create(CreateArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var indexRequest = new CreateIndexDto();
            var indexMatch = FieldsRegex().Match(arguments.Fields);

            var groups = indexMatch.Groups.OfType<Group>();

            var fieldNames = groups.Where(x => x.Name == "Field").ToList();
            var fieldOrders = groups.Where(x => x.Name == "Order").ToList();

            static SortOrder ParseOrder(string order)
            {
                if (string.Equals(order, "asc", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(order, "ascending", StringComparison.OrdinalIgnoreCase))
                {
                    return SortOrder.Ascending;
                }

                return SortOrder.Descending;
            }

            for (var i = 0; i < fieldNames.Count; i++)
            {
                var order = ParseOrder(fieldOrders[i].Value);

                indexRequest.Fields ??= [];
                indexRequest.Fields.Add(new IndexFieldDto { Name = fieldNames[i].Value, Order = order });
            }

            if (arguments.Print)
            {
                log.WriteLine(indexRequest.JsonPrettyString());
                return;
            }

            await session.Client.Schemas.PostIndexAsync(arguments.Schema, indexRequest);

            log.Completed("Index creation scheduled. Will be created in the background.");
        }

        [Command("drop", Description = "Create a new index.")]
        public async Task Drop(DropArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            await session.Client.Schemas.DeleteIndexAsync(arguments.Schema, arguments.Index);

            log.Completed("Index creation scheduled. Will be created in the background.");
        }

        public sealed class ListArguments : AppArguments
        {
            [Operand("name", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Option('t', "table", Description = "Output as table.")]
            public bool Table { get; set; }

            public sealed class Validator : AbstractValidator<ListArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Schema).NotEmpty();
                }
            }
        }

        public sealed class CreateArguments : AppArguments
        {
            [Operand("name", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Operand("fields", Description = "The fields in the following format 'field1=ASC,field2=DESC'")]
            public string Fields { get; set; }

            [Option("print", Description = "Only print the request to see the fields to be created.")]
            public bool Print { get; set; }

            public sealed class Validator : AbstractValidator<CreateArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Schema).NotEmpty();
                    RuleFor(x => x.Fields).NotEmpty().Matches(FieldsRegex());
                }
            }
        }

        public sealed class DropArguments : AppArguments
        {
            [Operand("name", Description = "The name of the schema.")]
            public string Schema { get; set; }

            [Operand("index", Description = "The name of the index.")]
            public string Index { get; set; }

            public sealed class Validator : AbstractValidator<DropArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.Schema).NotEmpty();
                    RuleFor(x => x.Index).NotEmpty();
                }
            }
        }

        [GeneratedRegex("((?<Field>[a-z0-9-._]+)[\\s]*=[\\s]*(?<Order>ASC|DESC|ASCENDING|DESC)[\\s\\,\\;]*)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture)]
        private static partial Regex FieldsRegex();
    }
}
