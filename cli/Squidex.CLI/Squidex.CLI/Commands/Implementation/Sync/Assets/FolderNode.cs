// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public sealed record FolderNode(string Id, string Path)
    {
        public Dictionary<string, FolderNode> Children { get; } = new Dictionary<string, FolderNode>();

        public FolderNode Parent { get; set; }

        public FolderNode Add(FolderNode child, string name)
        {
            Children[name] = child;
            child.Parent = this;

            return child;
        }
    }
}
