// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Squidex.CLI
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        static JsonHelper()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public static string JsonPrettyString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, SerializerSettings);
        }

        public static string JsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, SerializerSettings);
        }
    }
}
