// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands
{
    public static class Extensions
    {
        public static async Task<long> CreateFieldAsync(this ISchemasClient schemasClient, string app, string schema, FieldDto field)
        {
            var request = new AddFieldDto { Name = field.Name, Properties = field.Properties, Partitioning = field.Partitioning };

            var created = await schemasClient.PostFieldAsync(app, schema, request);

            return long.Parse(created.Id);
        }

        public static bool JsonEquals<T>(this T lhs, T rhs)
        {
            var lhsJson = JsonConvert.SerializeObject(lhs);
            var rhsJson = JsonConvert.SerializeObject(rhs);

            return !string.Equals(lhsJson, rhsJson, StringComparison.Ordinal);
        }

        public static bool JsonEqualsOrNew<T>(this T lhs, T rhs) where T : class, new()
        {
            var lhsJson = JsonConvert.SerializeObject(lhs ?? new T());
            var rhsJson = JsonConvert.SerializeObject(rhs ?? new T());

            return !string.Equals(lhsJson, rhsJson, StringComparison.Ordinal);
        }

        public static bool StringEquals(this string lhs, string rhs)
        {
            return string.Equals(lhs ?? string.Empty, rhs ?? string.Empty, StringComparison.Ordinal);
        }

        public static bool BoolEquals(this bool lhs, bool? rhs)
        {
            return lhs == (rhs ?? false);
        }
    }
}
