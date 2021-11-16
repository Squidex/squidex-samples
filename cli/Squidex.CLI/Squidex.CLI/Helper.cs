// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Squidex.CLI
{
    public static class Helper
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();
        private static readonly JsonSerializer Serializer;

        static Helper()
        {
            SerializerSettings.Converters.Add(new StringEnumConverter());
            SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false
                }
            };
            SerializerSettings.Formatting = Formatting.Indented;

            Serializer = JsonSerializer.Create(SerializerSettings);
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static async Task WriteJsonAsync<T>(this Stream stream, T value)
        {
            await using (var streamWriter = new StreamWriter(stream))
            {
                Serializer.Serialize(streamWriter, value);
            }
        }

        public static async Task WriteJsonToFileAsync<T>(T value, string path)
        {
            await using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                await stream.WriteJsonAsync(value);
            }
        }

        public static Task WriteJsonAsync<T>(this TextWriter streamWriter, T value)
        {
            Serializer.Serialize(streamWriter, value);

            return Task.CompletedTask;
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
                var length = maxLength - 3;

                return value[..maxLength] + "...";
            }
        }
    }
}
