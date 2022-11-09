// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Runtime.Serialization;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary;

/// <summary>
/// Default exception for all API requests.
/// </summary>
[Serializable]
public class SquidexException : Exception
{
    /// <summary>
    /// The HTTP status code if available.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// The error details if available.
    /// </summary>
    public ErrorDto? Details { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SquidexException"/> class.
    /// </summary>
    public SquidexException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SquidexException"/> class with the message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code if available.</param>
    /// <param name="details">The error details if available.</param>
    public SquidexException(string message, int statusCode, ErrorDto? details)
        : base(message)
    {
        StatusCode = statusCode;

        Details = details;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SquidexException"/> class with the message and inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code if available.</param>
    /// <param name="details">The error details if available.</param>
    /// <param name="inner">The inner exception.</param>
    public SquidexException(string message, int statusCode, ErrorDto? details, Exception inner)
        : base(message, inner)
    {
        StatusCode = statusCode;

        Details = details;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SquidexException"/> class with the serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext "/>  that contains contextual information about the source or destination.</param>
    protected SquidexException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        foreach (SerializationEntry entry in info)
        {
            switch (entry.Name)
            {
                case nameof(StatusCode) when entry.Value != null:
                    StatusCode = (int)entry.Value;
                    break;
                case nameof(Details) when entry.Value != null:
                    Details = (ErrorDto)entry.Value;
                    break;
            }
        }
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (StatusCode > 0)
        {
            info.AddValue(nameof(StatusCode), StatusCode);
        }

        if (Details != null)
        {
            info.AddValue(nameof(Details), Details, typeof(ErrorDto));
        }

        base.GetObjectData(info, context);
    }
}
