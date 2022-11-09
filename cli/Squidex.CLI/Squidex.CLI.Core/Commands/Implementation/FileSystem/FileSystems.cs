// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem.Default;
using Squidex.CLI.Commands.Implementation.FileSystem.Emedded;
using Squidex.CLI.Commands.Implementation.FileSystem.Git;
using Squidex.CLI.Commands.Implementation.FileSystem.Zip;

namespace Squidex.CLI.Commands.Implementation.FileSystem;

public static class FileSystems
{
    private const string AssemblyPrefix = "assembly://";

    public static async Task<IFileSystem> CreateAsync(string path, DirectoryInfo workingDirectory)
    {
        IFileSystem? fileSystem = null;

        if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
        {
            switch (uri.Scheme)
            {
                case "https" when uri.LocalPath.TrimEnd('/').EndsWith(".git", StringComparison.Ordinal):
                    var query = uri.ParseQueryString();

                    query.TryGetValue("folder", out var folder);

                    var repositoryUrl = $"{uri.Scheme}://{uri.Host}/{uri.LocalPath.TrimEnd('/')}";

                    fileSystem = new GitFileSystem(repositoryUrl, folder, query.ContainsKey("skip-pull"), workingDirectory);
                    break;
                case "file":
                    fileSystem = OpenFolder(uri.LocalPath);
                    break;
                case "assembly":
                    fileSystem = OpenAssembly(uri.LocalPath.Replace('/', '.'));
                    break;
            }
        }
        else
        {
            if (path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                fileSystem = OpenZip(path);
            }
            else if (path.StartsWith(AssemblyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                fileSystem = OpenAssembly(path[AssemblyPrefix.Length..]);
            }
            else
            {
                fileSystem = OpenFolder(path);
            }
        }

        if (fileSystem == null)
        {
            throw new InvalidOperationException($"Cannot open file system at {path}.");
        }

        await fileSystem.OpenAsync();

        return fileSystem;
    }

    private static IFileSystem OpenAssembly(string path)
    {
        var cleanedPath = path.Replace('/', '.');

        return new EmbeddedFileSystem(typeof(FileSystems).Assembly, cleanedPath);
    }

    private static IFileSystem OpenFolder(string path)
    {
        var directory = Directory.CreateDirectory(path);

        return new DefaultFileSystem(directory);
    }

    private static IFileSystem OpenZip(string path)
    {
        var file = new FileInfo(path);

        Directory.CreateDirectory(file.Directory!.FullName);

        return new ZipFileSystem(file);
    }
}
