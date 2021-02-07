// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public sealed class EnrichedUsageExceededEvent : EnrichedEvent
  {
    public long CallsCurrent { get; set; }

    public long CallsLimit { get; set; }

    public override long Partition { get; set; }
  }
}
