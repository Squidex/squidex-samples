// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.FileSystem;

public sealed class FilePath(params string[] elements)
{
    public static readonly FilePath Root = new FilePath(string.Empty);

    public string[] Elements { get; } = elements;

    public static FilePath Create(string path)
    {
        return new FilePath(path.Split('/', '\\'));
    }

    public FilePath Combine(FilePath path)
    {
        return new FilePath(Elements.Concat(path.Elements).ToArray());
    }

    public static implicit operator FilePath(string path)
    {
        return Create(path);
    }

    public override string ToString()
    {
        return Path.Combine(Elements);
    }
}
