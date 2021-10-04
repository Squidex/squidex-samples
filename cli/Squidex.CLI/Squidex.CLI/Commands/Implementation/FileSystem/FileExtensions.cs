// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;
using System.Linq;

namespace Squidex.CLI.Commands.Implementation.FileSystem
{
    public static class FileExtensions
    {
        public static string ReadAllText(this IFile file)
        {
            using (var stream = file.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string LocalFolderPath(this IFile file)
        {
            var elements = file.FullLocalName.Split('/', '\\');

            return Path.Combine(elements.SkipLast(1).ToArray());
        }
    }
}
