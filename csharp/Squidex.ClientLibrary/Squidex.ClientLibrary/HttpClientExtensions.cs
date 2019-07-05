// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Squidex.ClientLibrary
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializer JsonSerializer = JsonSerializer.CreateDefault();
        private static readonly JsonSerializerSettings PascalCasing = new JsonSerializerSettings();
        private static readonly JsonSerializerSettings CamelCasing = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static HttpContent ToContent<T>(this T value)
        {
            var serializerSettings =
                value.GetType().GetCustomAttribute<KeepCasingAttribute>() != null ?
                    PascalCasing :
                    CamelCasing;

            var json = JsonConvert.SerializeObject(value, Formatting.Indented, serializerSettings);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }

        public static MultipartFormDataContent ToContent(this Stream stream, string name, string mimeType)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotNullOrEmpty(mimeType, nameof(mimeType));
            Guard.NotNull(stream, nameof(stream));

            var streamContent = new StreamContent(stream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            var requestContent = new MultipartFormDataContent
            {
                { streamContent, "file", name }
            };

            return requestContent;
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            {
                using (var textReader = new StreamReader(stream))
                {
                    using (var streamReader = new JsonTextReader(textReader))
                    {
                        return JsonSerializer.Deserialize<T>(streamReader);
                    }
                }
            }
        }
    }
}
