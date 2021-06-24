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
using Squidex.CLI.Commands.Implementation.FileSystem;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public static class Extensions
    {
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

        public static Task WriteWithSchemaAs<T>(this JsonHelper jsonHelper, IFileSystem fs, FilePath path, object sample, string schema) where T : class
        {
            var file = fs.GetFile(path);

            return WriteWithSchemaAs<T>(jsonHelper, file, sample, schema);
        }

        public static Task WriteWithSchemaAs<T>(this JsonHelper jsonHelper, IFile file, object sample, string schema) where T : class
        {
            using (var stream = file.OpenWrite())
            {
                jsonHelper.WriteAs<T>(sample, stream, $"./{schema}.json");
            }

            return Task.CompletedTask;
        }

        public static Task WriteWithSchema<T>(this JsonHelper jsonHelper, IFileSystem fs, FilePath path, T sample, string schema) where T : class
        {
            var file = fs.GetFile(path);

            return WriteWithSchema(jsonHelper, file, sample, schema);
        }

        public static Task WriteWithSchema<T>(this JsonHelper jsonHelper, IFile file, T sample, string schema) where T : class
        {
            using (var stream = file.OpenWrite())
            {
                jsonHelper.Write(sample, stream, $"./{schema}.json");
            }

            return Task.CompletedTask;
        }

        public static Task WriteJsonSchemaAsync<T>(this JsonHelper jsonHelper, IFileSystem fs, FilePath path)
        {
            var file = fs.GetFile(new FilePath("__json").Combine(path));

            return WriteJsonSchemaAsync<T>(jsonHelper, file);
        }

        public static Task WriteJsonSchemaAsync<T>(this JsonHelper jsonHelper, IFile file)
        {
            using (var stream = file.OpenWrite())
            {
                jsonHelper.WriteSchema<T>(stream);
            }

            return Task.CompletedTask;
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
