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
using System.Threading.Tasks;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public static class Extensions
    {
        public static bool HasDistinctNames<T>(this ICollection<T> source, Func<T, string> selector)
        {
            return source.Select(selector).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count() == source.Count;
        }

        public static Task WriteWithSchemaAs<T>(this JsonHelper jsonHelper, DirectoryInfo directory, string path, object sample, string schema) where T : class
        {
            var fileInfo = GetFile(directory, path);

            using (var stream = fileInfo.Open(FileMode.Create))
            {
                jsonHelper.WriteAs<T>(sample, stream, $"./{schema}.json");
            }

            return Task.CompletedTask;
        }

        public static Task WriteWithSchema<T>(this JsonHelper jsonHelper, DirectoryInfo directory, string path, T sample, string schema) where T : class
        {
            var fileInfo = GetFile(directory, path);

            using (var stream = fileInfo.Open(FileMode.Create))
            {
                jsonHelper.Write(sample, stream, $"./{schema}.json");
            }

            return Task.CompletedTask;
        }

        public static Task WriteJsonSchemaAsync<T>(this JsonHelper jsonHelper, DirectoryInfo directory, string path)
        {
            var fileInfo = GetFile(directory, Path.Combine("__json", path));

            using (var stream = fileInfo.Open(FileMode.Create))
            {
                jsonHelper.WriteSchema<T>(stream);
            }

            return Task.CompletedTask;
        }

        private static FileInfo GetFile(DirectoryInfo directory, string path)
        {
            var fileInfo = new FileInfo(Path.Combine(directory.FullName, path));

            if (!fileInfo.Directory.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            return fileInfo;
        }
    }
}
