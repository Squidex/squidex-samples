// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public abstract class EnrichedUserEventBase : EnrichedEvent
  {
    public string Actor { get; set; }

    public string ActorId
    {
      get
      {
        return Actor == null ? null : Actor.Contains(":") ? Actor.Split(':')[1] : null;
      }
    }

    public string ActorType
    {
      get
      {
        return Actor == null ? null : Actor.Contains(":") ? Actor.Split(':')[0] : null;
      }
    }
  }
}
