// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using System.Text;

#pragma warning disable MA0089 // Optimize string method usage

namespace Squidex.ClientLibrary.Utils;

internal sealed class Query
{
    private StringBuilder? sb;

    public static Query Create()
    {
        return new Query();
    }

    public Query Append<T>(string key, T? value) where T : struct
    {
        if (!value.HasValue)
        {
            return this;
        }

        return Append(key, value.Value);
    }

    public Query Append<T>(string key, T value)
    {
        if (Equals(value, default(T)))
        {
            return this;
        }

        if (Equals(value, true))
        {
            return AppendCore(key, "true");
        }

        var formatted = Convert.ToString(value, CultureInfo.InvariantCulture);
        return AppendCore(key, formatted);
    }

    public Query AppendMany<T>(string key, IReadOnlyCollection<T>? values)
    {
        if (values == null || values.Count == 0)
        {
            return this;
        }

        var formatted = string.Join(",", values);
        return AppendCore(key, formatted);
    }

    public Query Append(string key, string? value, bool escape = false)
    {
        return AppendCore(key, value, escape);
    }

    private Query AppendCore(string key, string? value, bool escape = false)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return this;
        }

        sb ??= new StringBuilder();

        if (sb.Length > 0)
        {
            sb.Append('&');
        }
        else
        {
            sb.Append('?');
        }

        sb.Append(key);
        sb.Append('=');

        if (escape)
        {
            sb.Append('"');
            sb.Append(Uri.EscapeDataString(value));
            sb.Append('"');
        }
        else
        {
            sb.Append(Uri.EscapeDataString(value));
        }

        return this;
    }

    public override string ToString()
    {
        return sb?.ToString() ?? string.Empty;
    }
}
