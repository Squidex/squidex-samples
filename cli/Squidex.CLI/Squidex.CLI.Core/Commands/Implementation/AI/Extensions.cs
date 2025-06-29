// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net;
using System.Text;
using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;

namespace Squidex.CLI.Commands.Implementation.AI;

public static class Extensions
{
    public static string FormatError(this Error error, HttpStatusCode httpStatusCode)
    {
        var sb = new StringBuilder();

        void AddPart(string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (sb.Length > 0)
            {
                sb.Append(", ");
            }

            sb.Append($"{key}: {value}");
        }

        AddPart(nameof(Error.Code), error.Code);
        AddPart(nameof(Error.Type), error.Type);
        AddPart(nameof(Error.Message), error.Message);

        if (sb.Length == 0)
        {
            sb.Append("Unknown error");
        }

        if (httpStatusCode != default)
        {
            sb.Append($", HttpStatus: {httpStatusCode}");
        }

        return sb.ToString();
    }
}
