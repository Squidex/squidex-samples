// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary;

/// <summary>
/// Helper class to store folder information in memory and to resolve folder IDs by paths and vice versa.
/// </summary>
public sealed class AssetFolderTree
{
    private static readonly char[] TrimChars = { '/', '\\', ' ', '.' };
    private static readonly char[] SplitChars = { '/', '\\' };
    private static readonly string RootId = Guid.Empty.ToString();
    private readonly Dictionary<string, AssetFolderNode> nodes = new Dictionary<string, AssetFolderNode>();
    private readonly IAssetsClient assetsClient;
    private readonly string appName;
    private readonly AssetFolderNode rootNode = new AssetFolderNode(RootId, string.Empty, string.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetFolderTree"/> class.
    /// </summary>
    /// <param name="assetsClient">The assets client. Cannot be null.</param>
    /// <param name="appName">The app name. Cannot be null.</param>
    public AssetFolderTree(IAssetsClient assetsClient, string appName)
    {
        Guard.NotNull(assetsClient, nameof(assetsClient));
        Guard.NotNull(appName, nameof(appName));

        nodes[RootId] = rootNode;

        this.assetsClient = assetsClient;
        this.appName = appName;
    }

    /// <summary>
    /// Gets the path of a folder ID.
    /// </summary>
    /// <param name="id">The id to query. Can be null or empty for the root folder.</param>
    /// <returns>
    /// The path for the ID. Null for the root folder.
    /// </returns>
    public async Task<string?> GetPathAsync(string? id)
    {
        var node = await GetByIdAsync(id);

        return node?.Path;
    }

    /// <summary>
    /// Gets id of the path. If the folder do no not exist, they are created.
    /// </summary>
    /// <param name="path">The folder path. Can be null for the root folder.</param>
    /// <returns>
    /// The ID for the asset folder path. Null for the root folder.
    /// </returns>
    public async Task<string?> GetIdAsync(string? path)
    {
        var node = await GetByPathAsync(path);

        return node?.Id;
    }

    /// <summary>
    /// Get the folder information by the folder ID.
    /// </summary>
    /// <param name="id">The folder ID.</param>
    /// <param name="needsChildren">True if the list of children are needed for the folder node.</param>
    /// <returns>
    /// The folder information or null if the ID is null or empty.
    /// </returns>
    public async Task<AssetFolderNode?> GetByIdAsync(string? id, bool needsChildren = false)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return await QueryAsync(id!, needsChildren);
    }

    /// <summary>
    /// Get the folder information by the folder path.
    /// </summary>
    /// <param name="path">The folder oatg.</param>
    /// <returns>
    /// The folder information or null if the ID is null or empty.
    /// </returns>
    public async Task<AssetFolderNode?> GetByPathAsync(string? path)
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

        return current;
    }

    private async Task<AssetFolderNode> AddFolderAsync(AssetFolderNode current, string name)
    {
        var request = new CreateAssetFolderDto
        {
            FolderName = name,
            ParentId = current.Id
        };

        var folder = await assetsClient.PostAssetFolderAsync(appName, request);

        current = TryAdd(current, folder.Id, name);
        current.HasBeenQueried = true;

        return current;
    }

    private async Task<AssetFolderNode> QueryAsync(string id, bool needsChildren)
    {
        if (nodes.TryGetValue(id, out var node) && (node.HasBeenQueried || !needsChildren))
        {
            return node;
        }

        var folders = await assetsClient.GetAssetFoldersAsync(appName, id);

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

    private AssetFolderNode TryAdd(AssetFolderNode node, string id, string name)
    {
        if (node.Children.TryGetValue(name, out var child))
        {
            return child;
        }

        child = new AssetFolderNode(id, name, GetPath(node, name));

        nodes[id] = child;
        node.Add(child, name);

        return child;
    }

    private static string GetPath(AssetFolderNode node, string name)
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
