// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================


#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public sealed record FolderNode(string Id, string Path)
    {
        public static readonly string RootId = Guid.Empty.ToString();

        public Dictionary<string, FolderNode> Children { get; } = new Dictionary<string, FolderNode>();

        public FolderNode Parent { get; set; }

        public bool HasBeenQueried { get; set; }

        public void Add(FolderNode child, string name)
        {
            Children[name] = child;
            child.Parent = this;
        }
    }
}
