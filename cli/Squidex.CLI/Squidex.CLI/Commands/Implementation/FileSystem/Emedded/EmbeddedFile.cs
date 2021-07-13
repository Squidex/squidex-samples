// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Reflection;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Emedded
{
    public sealed class EmbeddedFile : IFile
    {
        private readonly Assembly assembly;

        public string FullName { get; }

        public string Name { get; }

        public bool Exists => CanOpen();

        public EmbeddedFile(Assembly assembly, string name, string fullName)
        {
            this.assembly = assembly;

            Name = name;

            FullName = fullName;
        }

        public Stream OpenRead()
        {
            var stream = assembly.GetManifestResourceStream(FullName);

            if (stream == null)
            {
                throw new FileNotFoundException(null, FullName);
            }

            return stream;
        }

        public Stream OpenWrite()
        {
            throw new NotSupportedException();
        }

        public void Delete()
        {
            throw new NotSupportedException();
        }

        private bool CanOpen()
        {
            var stream = assembly.GetManifestResourceStream(FullName);

            if (stream != null)
            {
                stream.Dispose();
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
