// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.EnrichedEvents;

/// <summary>
/// Envelope which contains the generated events.
/// </summary>
public class EnrichedEventEnvelope
{
    /// <summary>
    /// Type of the event.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// When the event has been generated.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The payload of the evnet.
    /// </summary>
    public EnrichedEvent Payload { get; set; }

    /// <summary>
    /// Utils to deserialize an Envelope.
    /// </summary>
    /// <param name="json">The string to be deserialized.</param>
    /// <param name="options">The Squidex options.</param>
    /// <returns>
    /// The enriched event.
    /// </returns>
    public static EnrichedEventEnvelope FromJson(string json, SquidexOptions options)
    {
        return json.FromJsonWithTypes<EnrichedEventEnvelope>(options);
    }
}
