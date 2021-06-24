// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO;

namespace Squidex.CLI.Commands.Implementation.FileSystem
{
    public interface IFile
    {
        string FullName { get; }

        string Name { get; }

        bool Exists { get; }

        Stream OpenRead();

        Stream OpenWrite();

        void Delete();
    }
}
