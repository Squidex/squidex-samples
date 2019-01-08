// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Squidex.CLI
{
    public static class Helper
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        static Helper()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static string JsonPrettyString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, SerializerSettings);
        }

        public static string JsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, SerializerSettings);
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (value.Length <= maxLength)
            {
                return value;
            }
            else
            {
                return value.Substring(0, maxLength - 3) + "...";
            }
        }
    }
}
