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

        public ZipFileSystem(FileInfo fileInfo)
        {
            zipArchive = new ZipArchive(fileInfo.Open(FileMode.OpenOrCreate), ZipArchiveMode.Update);

            this.fileInfo = fileInfo;
        }

        public IFile GetFile(FilePath path)
        {
            var relativePath = Path.Combine(path.Elements.ToArray());

            return new ZipFile(zipArchive, relativePath, path.Elements.Last(), FullName);
        }

        public IEnumerable<IFile> GetFiles(FilePath path, string extension)
        {
            var relativePath = Path.Combine(path.Elements.ToArray());

            foreach (var entry in zipArchive.Entries)
            {
                if (entry.FullName.StartsWith(relativePath, StringComparison.OrdinalIgnoreCase) && entry.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new ZipFile(zipArchive, entry.FullName, entry.Name, FullName);
                }
            }
        }

        public void Dispose()
        {
            zipArchive.Dispose();
        }
    }
}
