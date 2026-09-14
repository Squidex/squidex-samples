// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net;
using System.Text;
using Betalgo.Ranul.OpenAI.Contracts.Responses.Base;
using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;

namespace Squidex.CLI.Commands.Implementation.AI;

public static class Extensions
{
    public static string FormatError(this Error error, HttpStatusCode httpStatusCode)
    {
        return FormatError(error.Code, error.Type, error.Message, httpStatusCode);
    }

    public static string FormatError(this ResponseError error, HttpStatusCode httpStatusCode)
    {
        return FormatError(error.Code, error.Type, error.Message, httpStatusCode);
    }

    private static string FormatError(string? code, string? type, string? message, HttpStatusCode httpStatusCode)
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

        AddPart("Code", code);
        AddPart("Type", type);
        AddPart("Message", message);

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
