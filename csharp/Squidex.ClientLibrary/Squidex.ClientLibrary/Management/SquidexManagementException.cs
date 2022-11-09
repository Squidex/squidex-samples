// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable RECS0096 // Type parameter is never used

namespace Squidex.ClientLibrary.Management;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public partial class SquidexManagementException<TResult>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Result}\n{base.ToString()}";
    }
}
