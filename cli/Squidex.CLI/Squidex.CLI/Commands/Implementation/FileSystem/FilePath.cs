// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;

namespace Squidex.CLI.Commands.Implementation.FileSystem
{
    public sealed class FilePath
    {
        public IEnumerable<string> Elements { get; }

        public FilePath(params string[] elements)
        {
            Elements = elements;
        }

        public FilePath Combine(FilePath path)
        {
            return new FilePath(Elements.Concat(path.Elements).ToArray());
        }
    }
}
