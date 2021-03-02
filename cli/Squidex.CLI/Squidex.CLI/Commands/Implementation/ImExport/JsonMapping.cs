// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.ImExport
{
    public sealed class JsonMapping : List<(string Name, JsonPath Path)>
    {
        public static JsonMapping ForJson2Csv(string fields)
        {
            fields ??= string.Empty;

            var result = new JsonMapping();

            foreach (var item in fields.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = item.Split('=');

                static JsonPath GetPath(string value)
                {
                    var path = JsonPath.Parse(value);

                    if (path.Count == 2 && (IsData(path) || IsDataDraft(path)))
                    {
                        path.Add(("iv", -1));
                    }

                    return path;
                }

                switch (parts.Length)
                {
                    case 1:
                        result.Add((parts[0], GetPath(parts[0])));
                        break;
                    case 2:
                        result.Add((parts[0], GetPath(parts[1])));
                        break;
                    default:
                        throw new SquidexException("Field definition not valid.");
                }
            }

            if (result.Count == 0)
            {
                throw new SquidexException("Field definition not valid.");
            }

            return result;
        }

        public static JsonMapping ForCsv2Json(string fields)
        {
            fields ??= string.Empty;

            var result = new JsonMapping();

            foreach (var item in fields.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = item.Split('=');

                static JsonPath GetPath(string value)
                {
                    var path = JsonPath.Parse(value);

                    if (path.Count == 1)
                    {
                        path.Add(("iv", -1));
                    }

                    return path;
                }

                switch (parts.Length)
                {
                    case 1:
                        result.Add((parts[0], GetPath(parts[0])));
                        break;
                    case 2:
                        result.Add((parts[1], GetPath(parts[0])));
                        break;
                    default:
                        throw new SquidexException("Field definition not valid.");
                }
            }

            if (result.Count == 0)
            {
                throw new SquidexException("Field definition not valid.");
            }

            return result;
        }

        private static bool IsDataDraft(JsonPath path)
        {
            return string.Equals(path[0].Key, "DataDraft", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsData(JsonPath path)
        {
            return string.Equals(path[0].Key, "Data", StringComparison.OrdinalIgnoreCase);
        }
    }
}
