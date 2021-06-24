// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using System.IO.Compression;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Zip
{
    public sealed class ZipFile : IFile
    {
        private readonly ZipArchive archive;
        private readonly string archivePath;
        private ZipArchiveEntry entry;

        public string FullName { get; }

        public string Name { get; }

        public bool Exists => entry != null;

        public ZipFile(ZipArchive archive, string archivePath, string name, string filePath)
        {
            entry = archive.GetEntry(archivePath);

            this.archive = archive;
            this.archivePath = archivePath;

            Name = name;

            FullName = $"{filePath}//{archivePath}";
        }

        public void Delete()
        {
            entry.Delete();
        }

        public Stream OpenRead()
        {
            if (entry == null)
            {
                throw new FileNotFoundException(null, archivePath);
            }

            return entry.Open();
        }

        public Stream OpenWrite()
        {
            if (entry == null)
            {
                entry = archive.CreateEntry(archivePath);
            }

            return entry.Open();
        }
    }
}
