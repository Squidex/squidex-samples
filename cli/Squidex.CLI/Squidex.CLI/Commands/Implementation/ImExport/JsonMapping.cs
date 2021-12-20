// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Squidex.CLI.Commands.Implementation.ImExport
{
    public sealed class JsonMapping : List<(string Name, JsonPath Path, string Format)>
    {
        private static readonly Regex FormatRegex = new Regex("(?<Lhs>[^\\/=]*)(=(?<Rhs>[^\\/]*))?(\\/(?<Format>.*))?", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public static JsonMapping ForJson2Csv(string? fields)
        {
            fields ??= string.Empty;

            var result = new JsonMapping();

            foreach (var field in fields.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(field))
                {
                    continue;
                }

                static JsonPath GetPath(string value)
                {
                    var path = JsonPath.Parse(value);

                    if (path.Count == 2 && (IsData(path) || IsDataDraft(path)))
                    {
                        path.Add(("iv", -1));
                    }

                    return path;
                }

                var match = FormatRegex.Match(field);

                if (match.Success)
                {
                    var name = match.Groups["Lhs"].Value;
                    var path = name;

                    if (match.Groups["Rhs"].Success)
                    {
                        path = match.Groups["Rhs"].Value;
                    }

                    result.Add((name, GetPath(path), GetFormat(match)));
                }
                else
                {
                    throw new CLIException("Field definition not valid.");
                }
            }

            if (result.Count == 0)
            {
                throw new CLIException("Field definition not valid.");
            }

            return result;
        }

        public static JsonMapping ForCsv2Json(string? fields)
        {
            fields ??= string.Empty;

            var result = new JsonMapping();

            foreach (var field in fields.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(field))
                {
                    continue;
                }

                static JsonPath GetPath(string value)
                {
                    var path = JsonPath.Parse(value);

                    if (path.Count == 1)
                    {
                        path.Add(("iv", -1));
                    }

                    return path;
                }

                var match = FormatRegex.Match(field);

                if (match.Success)
                {
                    var name = match.Groups["Lhs"].Value;
                    var path = name;

                    if (match.Groups["Rhs"].Success)
                    {
                        name = match.Groups["Rhs"].Value;
                    }

                    result.Add((name, GetPath(path), GetFormat(match)));
                }
                else
                {
                    throw new CLIException("Field definition not valid.");
                }
            }

            if (result.Count == 0)
            {
                throw new CLIException("Field definition not valid.");
            }

            return result;
        }

        private static string GetFormat(Match match)
        {
            var format = "json";

            if (match.Groups["Format"].Success)
            {
                format = match.Groups["Format"].Value;
            }

            return format;
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
