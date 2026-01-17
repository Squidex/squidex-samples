// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Squidex.ClientLibrary.EnrichedEvents;

namespace Squidex.ClientLibrary.Utils;

/// <summary>
/// Extension methods to deal with JSON.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Converts a value to a <see cref="HttpContent"/> instance which contains a JSON body.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The created <see cref="HttpContent"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public static HttpContent ToContent<T>(this T value, SquidexOptions options)
    {
        var json = value.ToJson(options);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return content;
    }

    /// <summary>
    /// Converts a stream to a <see cref="HttpContent"/> instance which contains a JSON body.
    /// </summary>
    /// <param name="stream">The stream to convert.</param>
    /// <param name="name">The file name.</param>
    /// <param name="mimeType">The mime type.</param>
    /// <returns>
    /// The created <see cref="HttpContent"/> instance.
    /// </returns>
    public static HttpContent ToContent(this Stream stream, string name, string mimeType)
    {
        var streamContent = new StreamContent(stream);

        streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

        var requestContent = new MultipartFormDataContent
        {
            { streamContent, "file", name },
        };

        return requestContent;
    }

    /// <summary>
    /// Converts a value to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The JSON string.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public static string ToJson<T>(this T value, SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        var json = JsonConvert.SerializeObject(value, Formatting.Indented, options.SerializerSettings);

        return json;
    }

    /// <summary>
    /// Read a value from a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The JSON string.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The deserialized value.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public static T FromJson<T>(this string value, SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        var json = JsonConvert.DeserializeObject<T>(value, options.SerializerSettings)!;

        return json;
    }

    /// <summary>
    /// Read a value from a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The JSON string.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The deserialized value.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public static T FromJson<T>(this StringReader value, SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        using var jsonReader = new JsonTextReader(value);

        var jsonSerializer = JsonSerializer.CreateDefault(options.SerializerSettings);

        return jsonSerializer.Deserialize<T>(jsonReader)!;
    }

    /// <summary>
    /// Read a value from a JSON string and uses explicit type name handling.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The JSON string.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The deserialized value.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public static T FromJsonWithTypes<T>(this string value, SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        var serializerSettings = new JsonSerializerSettings(options.SerializerSettings)
        {
            SerializationBinder = new EnrichedEventSerializationBinder(),
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        var json = JsonConvert.DeserializeObject<T>(value, serializerSettings)!;

        return json;
    }

    /// <summary>
    /// Read a JSON value from a <see cref="HttpContent"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="content">The content to read from.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    /// The deserialized value.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content, SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        var serializer = JsonSerializer.CreateDefault(options.SerializerSettings);

#if NET8_0_OR_GREATER
        await using var stream = await content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);
        return serializer.Deserialize<T>(jsonReader);
#else
        using var stream = await content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);
        return serializer.Deserialize<T>(jsonReader);
#endif
    }
}
