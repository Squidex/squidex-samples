// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Squidex.CLI.Commands.Implementation
{
    public static class Extensions
    {
        public static bool JsonEquals<T, TOther>(this T lhs, TOther rhsOther)
        {
            var rhsOtherJson = JsonConvert.SerializeObject(rhsOther);

            var rhs = JsonConvert.DeserializeObject<T>(rhsOtherJson);

            var lhsJson = JsonConvert.SerializeObject(lhs);
            var rhsJson = JsonConvert.SerializeObject(rhs);

            return lhsJson == rhsJson;
        }

        public static bool SetEquals(this IEnumerable<string> lhs, IEnumerable<string> rhs)
        {
            if (rhs == null)
            {
                return false;
            }

            return new HashSet<string>(lhs).SetEquals(rhs);
        }

        public static bool HasDistinctNames<T>(this ICollection<T> source, Func<T, string> selector)
        {
            return source.Select(selector).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count() == source.Count;
        }

        public static void Foreach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var index = 0;

            foreach (var item in source)
            {
                action(item, index);
                index++;
            }
        }

        public static async Task Foreach<T>(this IEnumerable<T> source, Func<T, int, Task> action)
        {
            var index = 0;

            foreach (var item in source)
            {
                await action(item, index);
                index++;
            }
        }

        public static string Sha256Base64(this string value)
        {
            return Sha256Base64(Encoding.UTF8.GetBytes(value));
        }

        public static string Sha256Base64(this byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                var bytesHash = sha.ComputeHash(bytes);

                var result = Convert.ToBase64String(bytesHash);

                return result;
            }
        }
    }
}
