// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public sealed class EnrichedAssetEvent : EnrichedUserEventBase, IEnrichedEntityEvent
  {
    public EnrichedAssetEventType Type { get; set; }

    public string Id { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string CreatedBy { get; set; }

    public string LastModifiedBy { get; set; }

    public string MimeType { get; set; }

    public string FileName { get; set; }

    public long FileVersion { get; set; }

    public long FileSize { get; set; }

    public int? PixelWidth { get; set; }

    public int? PixelHeight { get; set; }

    public AssetType AssetType { get; set; }

    public bool IsImage { get; set; }

    public override long Partition { get; set; }

    public string CreatedById
    {
      get
      {
        return CreatedBy == null ? null : CreatedBy.Contains(":") ? CreatedBy.Split(':')[1] : null;
      }
    }

    public string CreatedByType
    {
      get
      {
        return CreatedBy == null ? null : CreatedBy.Contains(":") ? CreatedBy.Split(':')[0] : null;
      }
    }

    public string LastModifiedById
    {
      get
      {
        return LastModifiedBy == null ? null : LastModifiedBy.Contains(":") ? LastModifiedBy.Split(':')[1] : null;
      }
    }

    public string LastModifiedByType
    {
      get
      {
        return LastModifiedBy == null ? null : LastModifiedBy.Contains(":") ? LastModifiedBy.Split(':')[0] : null;
      }
    }
  }
}
