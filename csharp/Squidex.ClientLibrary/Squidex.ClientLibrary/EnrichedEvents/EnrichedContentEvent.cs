// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <inheritdoc />
    public sealed class EnrichedContentEvent<T> : EnrichedContentEvent
    {
        /// <summary>
        /// Data payload (updated).
        /// </summary>
        public new T Data { get; set; }

        /// <summary>
        /// Data payload (previous).
        /// </summary>
        public new T DataOld { get; set; }
    }

    /// <summary>
    /// Event on a content.
    /// </summary>
    public class EnrichedContentEvent : EnrichedSchemaEventBase, IEnrichedEntityEvent
    {
        /// <summary>
        /// Content event type.
        /// </summary>
        public EnrichedContentEventType Type { get; set; }

        /// <summary>
        /// Content id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// When the content has been created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// When the content has been modified.
        /// </summary>
        public DateTimeOffset LastModified { get; set; }

        /// <summary>
        /// Who has created the content.
        /// </summary>
        [JsonConverter(typeof(ActorConverter))]
        public Actor CreatedBy { get; set; }

        /// <summary>
        /// Who has modified the content.
        /// </summary>
        [JsonConverter(typeof(ActorConverter))]
        public Actor LastModifiedBy { get; set; }

        /// <summary>
        /// Data payload (updated).
        /// </summary>
        public JObject Data { get; set; }

        /// <summary>
        /// Data payload (previous).
        /// </summary>
        public JObject DataOld { get; set; }

        /// <summary>
        /// Status of the content.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Get a instance of EnrichedContentEvent where Data and DataOld have type T.
        /// </summary>
        /// <typeparam name="T">Type of Data and DataOld properties.</typeparam>
        /// <returns>EnrichedContentEvent instance where Data and DataOld have type T.</returns>
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

            if (Data != null)
            {
                obj.Data = (T)Data.ToObject(contentType);
            }

            if (DataOld != null)
            {
                obj.DataOld = (T)DataOld.ToObject(contentType);
            }

            return obj;
        }
    }
}
