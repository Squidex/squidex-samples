// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using Squidex.CLI.Commands.Implementation.FileSystem.Default;
using Squidex.CLI.Commands.Implementation.FileSystem.Zip;

namespace Squidex.CLI.Commands.Implementation.FileSystem
{
    public static class FileSystems
    {
        public static IFileSystem Create(string path)
        {
            if (path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var file = new FileInfo(path);

                Directory.CreateDirectory(file.Directory.FullName);

                return new ZipFileSystem(file);
            }
            else
            {
                var directory = Directory.CreateDirectory(path);

                return new DefaultFileSystem(directory);
            }
        }
    }
}
