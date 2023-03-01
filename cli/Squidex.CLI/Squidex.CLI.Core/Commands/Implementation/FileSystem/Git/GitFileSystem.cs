// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using LibGit2Sharp;
using Squidex.CLI.Commands.Implementation.FileSystem.Default;
using Squidex.CLI.Commands.Implementation.Utils;
using Squidex.Text;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Git;

public sealed class GitFileSystem : IFileSystem
{
    private readonly string sourceUrl;
    private readonly string? sourceDir;
    private DefaultFileSystem inner;
    private DirectoryInfo? cloneDirectory;

    public string FullName
    {
        get
        {
            EnsureOpened();

            return inner.FullName;
        }
    }

    public string Url
    {
        get => sourceUrl;
    }

    public string? Folder
    {
        get => sourceDir;
    }

    public GitFileSystem(Uri uri)
    {
        var query = uri.ParseQueryString();

        query.TryGetValue("folder", out var folder);

        sourceUrl = $"{uri.Scheme}://{uri.Host}/{uri.LocalPath.TrimEnd('/')}";
        sourceDir = folder;
    }

    public IFile GetFile(FilePath path)
    {
        EnsureOpened();

        return inner.GetFile(path);
    }

    public IEnumerable<IFile> GetFiles(FilePath path, string extension)
    {
        EnsureOpened();

        return inner.GetFiles(path, extension);
    }

    public Task OpenAsync()
    {
        var repositoryName = GetRepositoryName(sourceUrl);

        var repositoryPath = Path.Combine(Path.GetTempPath(), $"repository_{Path.GetRandomFileName()}_{repositoryName}");
        var repositoryFolder = Directory.CreateDirectory(repositoryPath);

        Repository.Clone(sourceUrl, repositoryFolder.FullName);

        if (!string.IsNullOrWhiteSpace(sourceDir))
        {
            repositoryFolder = repositoryFolder.GetDirectory(sourceDir);
        }

        inner = new DefaultFileSystem(repositoryFolder)
        {
            Readonly = true
        };

        cloneDirectory = repositoryFolder;

        return Task.CompletedTask;
    }

    private static string GetRepositoryName(string path)
    {
        string name;

        if (Uri.TryCreate(path, UriKind.Absolute, out var url))
        {
            name = url.LocalPath;
        }
        else
        {
            var dot = path.IndexOf(':', StringComparison.Ordinal);

            if (dot >= 0)
            {
                name = path[(dot + 1)..];
            }
            else
            {
                name = path;
            }
        }

        if (name.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
        {
            name = name[0..^4];
        }

        return name.Slugify();
    }

    private void EnsureOpened()
    {
        if (inner == null)
        {
            throw new InvalidOperationException("File system has not been opened yet.");
        }
    }

    public void Dispose()
    {
        try
        {
            DeleteDirectory(cloneDirectory?.FullName);
        }
        finally
        {
            inner?.Dispose();
        }
    }

    private static void DeleteDirectory(string? directoryPath)
    {
        if (directoryPath == null || !Directory.Exists(directoryPath))
        {
            return;
        }

        var directories = Directory.GetDirectories(directoryPath);

        foreach (var file in Directory.GetFiles(directoryPath))
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var dir in directories)
        {
            DeleteDirectory(dir);
        }

        File.SetAttributes(directoryPath, FileAttributes.Normal);

        Directory.Delete(directoryPath, false);
    }
}
