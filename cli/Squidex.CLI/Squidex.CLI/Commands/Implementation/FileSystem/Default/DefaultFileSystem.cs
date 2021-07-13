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

namespace Squidex.CLI.Commands.Implementation.FileSystem.Default
{
    public sealed class DefaultFileSystem : IFileSystem
    {
        private readonly DirectoryInfo directory;

        public string FullName => directory.FullName;

        public DefaultFileSystem(DirectoryInfo directory)
        {
            this.directory = directory;
        }

        public IFile GetFile(FilePath path)
        {
            string fullPath = GetFullPath(path);

            return new DefaultFile(new FileInfo(fullPath));
        }

        public IEnumerable<IFile> GetFiles(FilePath path, string extension)
        {
            var directory = GetFullPath(path);

            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, $"*{extension}", SearchOption.AllDirectories))
                {
                    var fileInfo = new FileInfo(file);

                    if (fileInfo.Exists && string.Equals(fileInfo.Extension, extension, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new DefaultFile(fileInfo);
                    }
                }
            }
        }

        private string GetFullPath(FilePath path)
        {
            return Path.Combine(directory.FullName, Path.Combine(path.Elements.ToArray()));
        }

        public void Dispose()
        {
        }
    }
}
