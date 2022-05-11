// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Squidex.ClientLibrary.EnrichedEvents;

namespace Squidex.ClientLibrary.Utils
{
    /// <summary>
    /// Extension methods to deal with JSON.
    /// </summary>
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings;
        private static readonly JsonSerializerSettings SerializerSettingsWithTypes;
        private static readonly JsonSerializer Serializer;

        static HttpClientExtensions()
        {
            SerializerSettings = new JsonSerializerSettings().SetupSquidex();
            SerializerSettingsWithTypes = new JsonSerializerSettings().SetupSquidex();
            SerializerSettingsWithTypes.SerializationBinder = new EnrichedEventSerializationBinder();
            SerializerSettingsWithTypes.TypeNameHandling = TypeNameHandling.Auto;

            Serializer = JsonSerializer.CreateDefault(SerializerSettings);
        }

        /// <summary>
        /// Converts the serializer settings to deal with squidex data.
        /// </summary>
        /// <param name="settings">The JSON settings.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public static JsonSerializerSettings SetupSquidex(this JsonSerializerSettings settings)
        {
            settings.ContractResolver = new JsonNullContractResolver();

            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new UTCIsoDateTimeConverter());

            return settings;
        }

        /// <summary>
        /// Converts a value to a <see cref="HttpContent"/> instance which contains a JSON body.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>
        /// The created <see cref="HttpContent"/> instance.
        /// </returns>
        public static HttpContent ToContent<T>(this T value)
        {
            var json = value.ToJson();

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
                { streamContent, "file", name }
            };

            return requestContent;
        }

        /// <summary>
        /// Converts a value to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>
        /// The JSON string.
        /// </returns>
        public static string ToJson<T>(this T value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.Indented, SerializerSettings);

            return json;
        }

        /// <summary>
        /// Read a value from a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The JSON string.</param>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static T FromJson<T>(this string value)
        {
            var json = JsonConvert.DeserializeObject<T>(value, SerializerSettings)!;

            return json;
        }

        /// <summary>
        /// Read a value from a JSON string and uses explicit type name handling.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The JSON string.</param>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static T FromJsonWithTypes<T>(this string value)
        {
            var json = JsonConvert.DeserializeObject<T>(value, SerializerSettingsWithTypes)!;

            return json;
        }

        /// <summary>
        /// Read a JSON value from a <see cref="HttpContent"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="content">The content to read from.</param>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content)
        {
#if NET5_0_OR_GREATER
            await using (var stream = await content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        return Serializer.Deserialize<T>(jsonReader);
                    }
                }
            }
#else
            using (var stream = await content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        return Serializer.Deserialize<T>(jsonReader);
                    }
                }
            }
#endif
        }
    }
}
