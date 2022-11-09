// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;

namespace Squidex.ClientLibrary.Management;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[Serializable]
public partial class ErrorDto
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Message);

        if (Details?.Count > 0)
        {
            if (!Message.EndsWith(".", StringComparison.OrdinalIgnoreCase) &&
                !Message.EndsWith(":", StringComparison.OrdinalIgnoreCase) &&
                !Message.EndsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append(':');
            }

            sb.Append(' ');
            sb.Append(string.Join(", ", Details));
        }

        return sb.ToString();
    }
}
