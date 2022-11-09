// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;

namespace Squidex.CLI.Commands.Implementation.ImExport;

public sealed class JsonPath : List<(string Key, int Index)>
{
    public static JsonPath Parse(string value)
    {
        var result = new JsonPath();

        var path = value.Split('.', StringSplitOptions.RemoveEmptyEntries);

        foreach (var element in path)
        {
            if (!int.TryParse(element, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
            {
                index = -1;
            }

            result.Add((element, index));
        }

        return result;
    }
}
