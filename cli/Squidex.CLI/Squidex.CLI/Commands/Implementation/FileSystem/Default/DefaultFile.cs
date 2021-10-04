// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Default
{
    public sealed class DefaultFile : IFile
    {
        private readonly FileInfo fileInfo;

        public string FullName => fileInfo.FullName;

        public string FullLocalName { get; }

        public string Name => fileInfo.Name;

        public bool Exists => fileInfo.Exists;

        public DefaultFile(FileInfo fileInfo, string fullLocalName)
        {
            this.fileInfo = fileInfo;

            FullLocalName = fullLocalName;
        }

        public void Delete()
        {
            fileInfo.Delete();
        }

        public Stream OpenRead()
        {
            return fileInfo.OpenRead();
        }

        public Stream OpenWrite()
        {
            Directory.CreateDirectory(fileInfo.Directory.FullName);

            return new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
