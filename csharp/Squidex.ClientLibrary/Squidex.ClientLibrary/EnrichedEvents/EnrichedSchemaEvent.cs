// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public sealed class EnrichedSchemaEvent : EnrichedSchemaEventBase, IEnrichedEntityEvent
  {
    public EnrichedSchemaEventType Type { get; set; }

    public string Id { get; set; }

    public override long Partition { get; set; }
  }
}
