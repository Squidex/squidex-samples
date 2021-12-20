// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Squidex.CLI.Commands.Implementation
{
    public static class Extensions
    {
        private static readonly Regex QueryRegex = new Regex(@"[?&](\w[\w.]*)=([^?&]+)");

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

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            T[]? bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                {
                    bucket = new T[size];
                }

                bucket[count++] = item;

                if (count != size)
                {
                    continue;
                }

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
            {
                Array.Resize(ref bucket, count);

                yield return bucket.Select(x => x);
            }
        }

        public static DirectoryInfo CreateDirectory(this DirectoryInfo directory, string name)
        {
            return Directory.CreateDirectory(Path.Combine(directory.FullName, name));
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo directory, string name)
        {
            return new DirectoryInfo(Path.Combine(directory.FullName, name));
        }

        public static FileInfo GetFile(this DirectoryInfo directory, string name)
        {
            return new FileInfo(Path.Combine(directory.FullName, name));
        }

        public static Dictionary<string, string> ParseQueryString(this Uri uri)
        {
            var match = QueryRegex.Match(uri.PathAndQuery);

            var paramaters = new Dictionary<string, string>();

            while (match.Success)
            {
                paramaters[match.Groups[1].Value] = match.Groups[2].Value;

                match = match.NextMatch();
            }

            return paramaters;
        }
    }
}
