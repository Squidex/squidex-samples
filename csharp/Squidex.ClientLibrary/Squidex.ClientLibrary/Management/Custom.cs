// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;

#pragma warning disable RECS0096 // Type parameter is never used

namespace Squidex.ClientLibrary.Management
{
    public partial class ErrorDto
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Message);

            if (Details?.Count > 0)
            {
                sb.Append(": ");
                sb.Append(string.Join(", ", Details));
            }

            return sb.ToString();
        }
    }

    public partial class SquidexManagementException<TResult>
    {
        public override string ToString()
        {
            return string.Format("{0}\n{1}", Result, base.ToString());
        }
    }
}
