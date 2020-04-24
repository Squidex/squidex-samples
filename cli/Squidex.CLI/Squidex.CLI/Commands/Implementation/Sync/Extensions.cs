﻿// ==========================================================================
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
using Newtonsoft.Json.Linq;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public static class Extensions
    {
        public static bool HasDistinctNames<T>(this ICollection<T> source, Func<T, string> selector)
        {
            return source.Select(selector).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count() == source.Count;
        }

        public static async Task WriteWithSchemaAs<T>(this JsonHelper jsonHelper, DirectoryInfo directory, string path, object sample, string schema)
        {
            var fileInfo = GetFile(directory, path);

            var json = jsonHelper.SerializeAs<T>(sample, $"./{schema}.json");

            await File.WriteAllTextAsync(fileInfo.FullName, json);
        }

        public static async Task WriteWithSchema<T>(this JsonHelper jsonHelper, DirectoryInfo directory, string path, T sample, string schema)
        {
            var fileInfo = GetFile(directory, path);

            var json = jsonHelper.SerializeWithSchema(sample, $"./{schema}.json");

            await File.WriteAllTextAsync(fileInfo.FullName, json);
        }

        public static async Task WriteJsonSchemaAsync<T>(this JsonHelper jsonHelper, DirectoryInfo directory, string path)
        {
            var fileInfo = GetFile(directory, Path.Combine("__json", path));

            var json = jsonHelper.SchemaString<T>();

            await File.WriteAllTextAsync(fileInfo.FullName, json);
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

        public static JObject ToJObject<T>(this object value) where T : class
        {
            return JObject.FromObject(value as T);
        }
    }
}
