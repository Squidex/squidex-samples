// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerSettings PascalCasing = CreateSerializer(new DefaultContractResolver());

        private static readonly JsonSerializerSettings CamelCasing = CreateSerializer(new CamelCasePropertyNamesContractResolver());

        private static JsonSerializerSettings CreateSerializer(DefaultContractResolver contractResolver)
        {
            var result = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };

            result.Converters.Add(new UTCIsoDateTimeConverter());

            return result;
        }

        public static HttpContent ToContent<T>(this T value)
        {
            var json = value.ToJson();

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }

        public static string ToJson<T>(this T value)
        {
            var serializerSettings =
                value.GetType().GetCustomAttribute<KeepCasingAttribute>() != null ?
                    PascalCasing :
                    CamelCasing;

            var json = JsonConvert.SerializeObject(value, Formatting.Indented, serializerSettings);

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
