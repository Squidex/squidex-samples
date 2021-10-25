// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Zip
{
    public sealed class ZipFileSystem : IFileSystem
    {
        private readonly ZipArchive zipArchive;
        private readonly FileInfo fileInfo;

        public string FullName => fileInfo.FullName;

        public bool CanAccessInParallel => false;

        public ZipFileSystem(FileInfo fileInfo)
        {
            zipArchive = new ZipArchive(fileInfo.Open(FileMode.OpenOrCreate), ZipArchiveMode.Update);

            this.fileInfo = fileInfo;
        }

        public IFile GetFile(FilePath path)
        {
            string relativePath = GetRelativePath(path);

            return new ZipFile(zipArchive, relativePath, path.Elements[^1], FullName);
        }

        public IEnumerable<IFile> GetFiles(FilePath path, string extension)
        {
            var relativePath = path.ToString();

            foreach (var entry in zipArchive.Entries)
            {
                // Entry is a folder.
                if (string.IsNullOrWhiteSpace(entry.Name))
                {
                    continue;
                }

                if (entry.FullName.StartsWith(relativePath, StringComparison.OrdinalIgnoreCase) && MatchsExtension(entry.Name, extension))
                {
                    yield return new ZipFile(zipArchive, entry.FullName, entry.Name, FullName);
                }
            }
        }

        private static bool MatchsExtension(string fullName, string extension)
        {
            if (extension == ".*")
            {
                return true;
            }

            return fullName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetRelativePath(FilePath path)
        {
            return Path.Combine(path.Elements.ToArray());
        }

        public void Dispose()
        {
            zipArchive.Dispose();
        }
    }
}
