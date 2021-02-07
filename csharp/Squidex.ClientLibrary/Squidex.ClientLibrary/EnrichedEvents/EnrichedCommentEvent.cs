// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public sealed class EnrichedCommentEvent : EnrichedUserEventBase
  {
    public string Text { get; set; }

    public Uri Url { get; set; }

    public override long Partition { get; set; }
  }
}
