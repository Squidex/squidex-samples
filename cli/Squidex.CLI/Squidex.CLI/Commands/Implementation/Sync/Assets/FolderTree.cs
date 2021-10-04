// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation.Sync.Assets
{
    public sealed class FolderTree
    {
        private static readonly char[] TrimChars = { '/', '\\', ' ', '.' };
        private static readonly char[] SplitChars = { '/', '\\' };
        private static readonly string RootId = Guid.Empty.ToString();
        private readonly Dictionary<string, FolderNode> nodes = new Dictionary<string, FolderNode>();
        private readonly ISession session;
        private readonly FolderNode rootNode = new FolderNode(RootId, string.Empty);

        public FolderTree(ISession session)
        {
            nodes[RootId] = rootNode;

            this.session = session;
        }

        public async Task<string> GetPathAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == RootId)
            {
                return null;
            }

            if (nodes.TryGetValue(id, out var current))
            {
                return current.Path;
            }

            var folder = await QueryAsync(id, false);

            return folder.Path;
        }

        public async Task<string> GetIdAsync(string path)
        {
            if (path == null || path.Equals(".", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            path = path.Trim(TrimChars);

            if (path.Length == 0)
            {
                return null;
            }

            var names = path.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

            var current = rootNode;

            foreach (var name in names)
            {
                if (current.Children.TryGetValue(name, out var child))
                {
                    current = child;
                    continue;
                }

                var node = await QueryAsync(current.Id, true);

                if (node.Children.TryGetValue(name, out child))
                {
                    current = child;
                    continue;
                }

                current = await AddFolderAsync(current, name);
            }

            return current.Id;
        }

        private async Task<FolderNode> AddFolderAsync(FolderNode current, string name)
        {
            var request = new CreateAssetFolderDto
            {
                FolderName = name,
                ParentId = current.Id
            };

            var folder = await session.Assets.PostAssetFolderAsync(session.App, request);

            current = TryAdd(current, folder.Id, name);
            current.HasBeenQueried = true;

            return current;
        }

        private async Task<FolderNode> QueryAsync(string id, bool needsChildren)
        {
            if (nodes.TryGetValue(id, out var node) && (node.HasBeenQueried || !needsChildren))
            {
                return node;
            }

            var folders = await session.Assets.GetAssetFoldersAsync(session.App, id);

            var current = rootNode;

            foreach (var folder in folders.Path)
            {
                current = TryAdd(current, folder.Id, folder.FolderName);
            }

            current.HasBeenQueried = true;

            foreach (var child in folders.Items)
            {
                TryAdd(current, child.Id, child.FolderName);
            }

            return current;
        }

        private FolderNode TryAdd(FolderNode node, string id, string name)
        {
            if (node.Children.TryGetValue(name, out var child))
            {
                return child;
            }

            child = new FolderNode(id, GetPath(node, name));

            nodes[id] = child;
            node.Add(child, name);

            return child;
        }

        private static string GetPath(FolderNode node, string name)
        {
            var path = node.Path;

            if (path.Length > 0)
            {
                path += '/';
            }

            path += name;

            return path;
        }
    }
}
