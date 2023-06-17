// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable RECS0096 // Type parameter is never used

namespace Squidex.ClientLibrary;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public partial class SquidexException<TResult>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SquidexException{TResult}"/> class with the message and status code.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="result">The result object.</param>
    public SquidexException(string message, int statusCode = 0, TResult? result = default)
        : this(message, statusCode, null, null, result, null)
    {
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Result}\n{base.ToString()}";
    }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public partial class SquidexException
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SquidexException"/> class with the message and status code.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="statusCode">The status code.</param>
    public SquidexException(string message, int statusCode = 0)
        : this(message, statusCode, null, null, null)
    {
    }
}
