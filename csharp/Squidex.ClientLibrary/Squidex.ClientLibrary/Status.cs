// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.ComponentModel;

namespace Squidex.ClientLibrary;

/// <summary>
/// Default status strings.
/// </summary>
[TypeConverter(typeof(StatusTypeConverter))]
public readonly struct Status : IEquatable<Status>, IComparable<Status>
{
    /// <summary>
    /// Content is Archived (soft-delete).
    /// </summary>
    public static readonly Status Archived = new Status("Archived");

    /// <summary>
    /// Content is not ready and not available in the API by default.
    /// </summary>
    public static readonly Status Draft = new Status("Draft");

    /// <summary>
    /// Content is ready and published.
    /// </summary>
    public static readonly Status Published = new Status("Published");

    private readonly string name;

    /// <summary>
    /// Name of the status.
    /// </summary>
    public string Name
    {
        get { return name ?? "Unknown"; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Status"/> struct.
    /// </summary>
    /// <param name="name">Status name.</param>
    public Status(string name)
    {
        this.name = name;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Status status && Equals(status);
    }

    /// <inheritdoc />
    public bool Equals(Status other)
    {
        return string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(Name);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }

    /// <inheritdoc />
    public int CompareTo(Status other)
    {
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    /// <summary>
    /// Operator to compare to status objects for equality.
    /// </summary>
    /// <param name="lhs">The left side of the operator.</param>
    /// <param name="rhs">The right side of the operator.</param>
    public static bool operator ==(Status lhs, Status rhs)
    {
        return lhs.Equals(rhs);
    }

    /// <summary>
    /// Operator to compare to status objects for inequality.
    /// </summary>
    /// <param name="lhs">The left side of the operator.</param>
    /// <param name="rhs">The right side of the operator.</param>
    public static bool operator !=(Status lhs, Status rhs)
    {
        return !lhs.Equals(rhs);
    }

    /// <summary>
    /// Operator to compare the status to a string.
    /// </summary>
    /// <param name="status">The status to convert.</param>
    public static implicit operator string(Status status)
    {
        return status.ToString();
    }
}
