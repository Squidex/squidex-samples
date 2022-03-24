// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using LibGit2Sharp;
using Squidex.CLI.Commands.Implementation.FileSystem.Default;
using Squidex.Text;

namespace Squidex.CLI.Commands.Implementation.FileSystem.Git
{
    public sealed class GitFileSystem : IFileSystem
    {
        private readonly string gitUrl;
        private readonly string? gitFolder;
        private readonly bool skipPull;
        private readonly DirectoryInfo workingDirectory;
        private DefaultFileSystem inner;

        public string FullName
        {
            get
            {
                EnsureOpened();

                return inner.FullName;
            }
        }

        public void Dispose()
        {
            inner?.Dispose();
        }

        public GitFileSystem(string gitUrl, string? gitFolder, bool skipPull, DirectoryInfo workingDirectory)
        {
            this.gitUrl = gitUrl;
            this.gitFolder = gitFolder;
            this.skipPull = skipPull;
            this.workingDirectory = workingDirectory;
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
            var repositoryName = GetRepositoryName(gitUrl);

            var repositoriesFolder = workingDirectory.CreateDirectory("repositories");
            var repositoryFolder = repositoriesFolder.GetDirectory(repositoryName);

            if (repositoryFolder.Exists)
            {
                if (!skipPull)
                {
                    using (var repository = new Repository(repositoryFolder.FullName))
                    {
                        var signature = new Signature("cli", "cli@squidex.io", DateTimeOffset.UtcNow);

                        LibGit2Sharp.Commands.Pull(repository, signature, new PullOptions());
                    }
                }
            }
            else
            {
                Repository.Clone(gitUrl, repositoryFolder.FullName);
            }

            if (!string.IsNullOrWhiteSpace(gitFolder))
            {
                repositoryFolder = repositoryFolder.GetDirectory(gitFolder);
            }

            inner = new DefaultFileSystem(repositoryFolder)
            {
                Readonly = true
            };

            return Task.CompletedTask;
        }

        private string GetRepositoryName(string path)
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
    }
}
