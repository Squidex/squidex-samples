// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Squidex.ClientLibrary.EnrichedEvents
{
  public sealed class EnrichedContentEvent<T> : EnrichedContentEvent
  {
    public new T Data { get; set; }

    public new T DataOld { get; set; }
  }

  public class EnrichedContentEvent : EnrichedSchemaEventBase, IEnrichedEntityEvent
  {
    public EnrichedContentEventType Type { get; set; }

    public string Id { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string CreatedBy { get; set; }

    public string LastModifiedBy { get; set; }

    public JObject Data { get; set; }

    public JObject DataOld { get; set; }

    public Status Status { get; set; }

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

    public EnrichedContentEvent<T> ToTyped<T>()
    {
      var contentType = typeof(T);

      if (contentType == null)
      {
        throw new ArgumentNullException(nameof(contentType));
      }

      var typedEvent = typeof(EnrichedContentEvent<>).MakeGenericType(contentType);

      var obj = (EnrichedContentEvent<T>)Activator.CreateInstance(typedEvent);

      foreach (var property in typeof(EnrichedContentEvent).GetProperties().Where(prop => prop.CanWrite))
      {
        property.SetValue(obj, property.GetValue(this));
      }

      if (this.Data != null)
      {
        obj.Data = (T)this.Data.ToObject(contentType);
      }

      if (this.DataOld != null)
      {
        obj.DataOld = (T)this.DataOld.ToObject(contentType);
      }

      return obj;
    }
  }
}