// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using CommandDotNet.Attributes;
using ConsoleTables;
using Squidex.CLI.Configuration;

namespace Squidex.CLI.Commands
{
    public partial class App
    {
        [ApplicationMetadata(Name = "schemas", Description = "Manage schemas.")]
        [SubCommand]
        public class Schemas
        {
            [InjectProperty]
            public IConfigurationService Configuration { get; set; }

            [ApplicationMetadata(Name = "view", Description = "List all schemas.")]
            public async Task View([Option] bool json)
            {
                var service = Configuration.GetClient();
                var client = service.CreateSchemasClient();

                var schemas = await client.GetSchemasAsync(service.App);

                if (json)
                {
                    Console.WriteLine(schemas.JsonPrettyString());
                }
                else
                {
                    var table = new ConsoleTable("Id", "Name", "Published", "LastUpdate");

                    foreach (var schema in schemas)
                    {
                        table.AddRow(schema.Id, schema.Name, schema.IsPublished, schema.LastModified);
                    }

                    table.Write(Format.Default);
                }
            }
        }
    }
}
