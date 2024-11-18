// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.IO.Compression;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Zip;

public sealed class ZipFile(ZipArchive archive, string archivePath, string name, string filePath) : IFile
{
    private ZipArchiveEntry? entry = archive.GetEntry(archivePath);

    public string FullName { get; } = $"{filePath}//{archivePath}";

    public string FullLocalName => archivePath;

    public string Name { get; } = name;

    public long Size
    {
        get => entry?.Length ?? 0;
    }

    public bool Exists
    {
        get => entry != null;
    }

    public void Delete()
    {
        entry?.Delete();
    }

    public Stream OpenRead()
    {
        if (entry == null)
        {
            throw new FileNotFoundException(null, archivePath);
        }

        return entry.Open();
    }

    public Stream OpenWrite()
    {
        if (entry == null)
        {
            entry = archive.CreateEntry(archivePath);
        }

        return entry.Open();
    }

    public override string ToString()
    {
        return FullName;
    }
}
