// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public sealed class EnrichedEventSerializationBinder : ISerializationBinder
  {
    public IList<Type> EnrichedEventTypes { get; set; } = new List<Type> {
      typeof(EnrichedCommentEvent),
      typeof(EnrichedContentEvent),
      typeof(EnrichedManualEvent),
      typeof(EnrichedAssetEvent),
      typeof(EnrichedSchemaEvent),
      typeof(EnrichedUsageExceededEvent)
    };

    public Type BindToType(string assemblyName, string typeName)
    {
      return EnrichedEventTypes.SingleOrDefault(t => t.Name == typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
      assemblyName = null;
      typeName = serializedType.Name;
    }
  }
}
