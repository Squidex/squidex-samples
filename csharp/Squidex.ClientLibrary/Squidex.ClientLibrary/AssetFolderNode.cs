// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// Represents an asset folder.
/// </summary>
public sealed record AssetFolderNode
{
    /// <summary>
    /// The ID of the root folder.
    /// </summary>
    public static readonly string RootId = Guid.Empty.ToString();

    private readonly Dictionary<string, AssetFolderNode> children = [];

    /// <summary>
    /// Gets the children of this folder.
    /// </summary>
    public IReadOnlyDictionary<string, AssetFolderNode> Children => children;

    /// <summary>
    /// Gets the parent folder.
    /// </summary>
    public AssetFolderNode? Parent { get; private set; }

    /// <summary>
    /// Gets the ID of this folder.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the name of this folder.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the full path of this folder.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Indicates whether the children of the folder have been queried already.
    /// </summary>
    public bool HasBeenQueried { get; internal set; }

    internal AssetFolderNode(string id, string name, string path)
    {
        Id = id;
        Path = path;
        Name = name;
    }

    internal void Add(AssetFolderNode child, string name)
    {
        children[name] = child;
        child.Parent = this;
    }
}
