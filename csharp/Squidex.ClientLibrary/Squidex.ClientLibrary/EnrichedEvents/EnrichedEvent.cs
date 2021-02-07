// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public abstract class EnrichedEvent
  {
    public string AppId { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string Name { get; set; }

    public long Version { get; set; }

    public abstract long Partition { get; set; }
    public string AppName
    {
      get
      {
        return AppId == null ? null : AppId.Contains(",") ? AppId.Split(',')[1] : null;
      }
    }

    public Guid AppGuid
    {
      get
      {
        return AppId == null ? default : AppId.Contains(",") ? Guid.Parse(AppId.Split(',')[0]) : default;
      }
    }
  }
}
