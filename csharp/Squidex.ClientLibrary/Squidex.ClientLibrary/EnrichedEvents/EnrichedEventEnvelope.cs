// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public class EnrichedEventEnvelope
  {
    public string Type { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public EnrichedEvent Payload { get; set; }
  }
}
