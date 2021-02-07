// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.ComponentModel;

namespace Squidex.ClientLibrary
{
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

    public string Name
    {
      get { return name ?? "Unknown"; }
    }

    public Status(string name)
    {
      this.name = name;
    }

    public override bool Equals(object obj)
    {
      return obj is Status status && Equals(status);
    }

    public bool Equals(Status other)
    {
      return string.Equals(Name, other.Name);
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }

    public override string ToString()
    {
      return Name;
    }

    public int CompareTo(Status other)
    {
      return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    public static bool operator ==(Status lhs, Status rhs)
    {
      return lhs.Equals(rhs);
    }

    public static bool operator !=(Status lhs, Status rhs)
    {
      return !lhs.Equals(rhs);
    }
  }
}
