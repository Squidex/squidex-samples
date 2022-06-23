// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Security.Cryptography;

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

        public static string? GetFileHash(this IFile file)
        {
            return GetFileHash(file, file.Name);
        }

        public static string? GetFileHash(this IFile file, string fileName)
        {
            if (file == null)
            {
                return null;
            }

            try
            {
                using (var fileStream = file.OpenRead())
                {
                    var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

                    var buffer = new byte[80000];
                    var bytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer)) > 0)
                    {
                        incrementalHash.AppendData(buffer, 0, bytesRead);
                    }

                    var fileHash = Convert.ToBase64String(incrementalHash.GetHashAndReset());

                    var hash = $"{fileHash}{fileName}{fileStream.Length}".Sha256Base64();

                    return hash;
                }
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
