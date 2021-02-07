// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public abstract class EnrichedSchemaEventBase : EnrichedUserEventBase
  {
    public string SchemaId { get; set; }
    public string SchemaName
    {
      get
      {
        return SchemaId == null ? null : SchemaId.Contains(",") ? SchemaId.Split(',')[1] : null;
      }
    }

    public Guid SchemaGuid
    {
      get
      {
        return SchemaId == null ? default : SchemaId.Contains(",") ? Guid.Parse(SchemaId.Split(',')[0]) : default;
      }
    }
  }
}
