// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Squidex.ClientLibrary
{
    public static class HttpClientExtensions
    {
        public static HttpContent ToContent<T>(this T value)
        {
            var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

            return content;
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            var jsonObject = JsonConvert.DeserializeObject<T>(jsonString);

            return jsonObject;
        }
    }
}
