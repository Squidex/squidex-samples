// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.FileSystem;

public sealed class FilePath
{
    public static readonly FilePath Root = new FilePath(string.Empty);

    public string[] Elements { get; }

    public FilePath(params string[] elements)
    {
        Elements = elements;
    }

    public static FilePath Create(string path)
    {
        return new FilePath(path.Split('/', '\\'));
    }

    public FilePath Combine(FilePath path)
    {
        return new FilePath(Elements.Concat(path.Elements).ToArray());
    }

    public override string ToString()
    {
        return Path.Combine(Elements);
    }
}
