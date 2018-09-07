// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Squidex.ClientLibrary
{
    public static class HttpClientExtensions
    {
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

            var content = new StringContent(JsonConvert.SerializeObject(value, Formatting.Indented, serializerSettings), Encoding.UTF8, "application/json");

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
