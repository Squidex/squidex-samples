// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Models
{
    public sealed class SchemaWithRefs<T> where T : class
    {
        public T Schema { get; set; }

        public Dictionary<string, string> ReferencedSchemas { get; set; }

        public SchemaWithRefs()
        {
        }

        public SchemaWithRefs(T schema)
        {
            Schema = schema;

            ReferencedSchemas = new Dictionary<string, string>();
        }

        public static SchemaWithRefs<T> Parse(string json)
        {
            try
            {
                var imported = JsonConvert.DeserializeObject<SchemaWithRefs<T>>(json);

                if (imported.Schema != null && imported.ReferencedSchemas != null)
                {
                    return imported;
                }

                return ParseDirectly(json);
            }
            catch
            {
                try
                {
                    return ParseDirectly(json);
                }
                catch (IOException ex)
                {
                    throw new SquidexException($"Cannot deserialize schema: {ex.Message}");
                }
            }
        }

        private static SchemaWithRefs<T> ParseDirectly(string json)
        {
            var schema = JsonConvert.DeserializeObject<T>(json);

            return new SchemaWithRefs<T>(schema);
        }
    }
}
