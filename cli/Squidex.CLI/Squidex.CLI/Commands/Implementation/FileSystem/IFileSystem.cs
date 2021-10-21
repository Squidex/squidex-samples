// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Squidex.CLI.Commands.Implementation.FileSystem
{
    public interface IFileSystem : IDisposable
    {
        string FullName { get; }

        bool CanWrite => true;

        bool CanAccessInParallel => false;

        IFile GetFile(FilePath path);

        IEnumerable<IFile> GetFiles(FilePath path, string extension);
    }
}
