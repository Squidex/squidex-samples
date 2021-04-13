// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Squidex.ClientLibrary.Utils
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerSettings SerializerSettings = CreateSerializer();

        private static JsonSerializerSettings CreateSerializer()
        {
            var result = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            result.Converters.Add(new StringEnumConverter());
            result.Converters.Add(new UTCIsoDateTimeConverter());

            return result;
        }

        public static HttpContent ToContent<T>(this T value)
        {
            var json = value.ToJson();

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }

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

        public static string ToJson<T>(this T value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.Indented, SerializerSettings);

            return json;
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<T>(jsonString);

            return jsonObject;
        }
    }
}
