// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Reflection;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Emedded;

public sealed class EmbeddedFile(Assembly assembly, string name, string fullName, string fullLocalName) : IFile
{
    public string FullName { get; } = fullName;

    public string FullLocalName { get; } = fullLocalName;

    public string Name { get; } = name;

    public long Size
    {
        get => GetSize();
    }

    public bool Exists
    {
        get => CanOpen();
    }

    public Stream OpenRead()
    {
        var stream = assembly.GetManifestResourceStream(FullName);

        return stream ?? throw new FileNotFoundException(null, FullName);
    }

    public Stream OpenWrite()
    {
        throw new NotSupportedException();
    }

    public void Delete()
    {
        throw new NotSupportedException();
    }

    private bool CanOpen()
    {
        var stream = assembly.GetManifestResourceStream(FullName);

        if (stream != null)
        {
            var canOpen = true;

            stream.Dispose();
            return canOpen;
        }

        return false;
    }

    private long GetSize()
    {
        var stream = assembly.GetManifestResourceStream(FullName);

        if (stream != null)
        {
            var size = stream.Length;

            stream.Dispose();
            return size;
        }

        return 0;
    }

    public override string ToString()
    {
        return FullName;
    }
}
