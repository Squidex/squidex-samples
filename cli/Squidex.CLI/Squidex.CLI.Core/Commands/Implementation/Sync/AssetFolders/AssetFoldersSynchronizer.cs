// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem;
using Squidex.ClientLibrary;

namespace Squidex.CLI.Commands.Implementation.Sync.AssetFolders
{
    public sealed class AssetFoldersSynchronizer : ISynchronizer
    {
        private const string Ref = "../__json/assetFolders";
        private readonly ILogger log;

        public int Order => -2000;

        public string Name => "AssetFolders";

        public AssetFoldersSynchronizer(ILogger log)
        {
            this.log = log;
        }

        public Task CleanupAsync(IFileSystem fs)
        {
            foreach (var file in GetFiles(fs))
            {
                file.Delete();
            }

            return Task.CompletedTask;
        }

        public async Task ExportAsync(ISyncService sync, SyncOptions options, ISession session)
        {
            var model = new AssetFoldersModel
            {
                Paths = new List<string>()
            };

            async Task QueryAsync(string id)
            {
                var node = await sync.Folders.GetByIdAsync(id, true);

                foreach (var child in node?.Children?.Values ?? Enumerable.Empty<AssetFolderNode>())
                {
                    model.Paths.Add(child.Path);

                    await QueryAsync(child.Id);
                }
            }

            await log.DoSafeAsync("Exporting folders", async () =>
            {
                await QueryAsync(AssetFolderNode.RootId);
            });

            await sync.WriteWithSchema(new FilePath("assetFolders/assetFolders.json"), model, Ref);
        }

        public Task DescribeAsync(ISyncService sync, MarkdownWriter writer)
        {
            var models =
                GetFiles(sync.FileSystem)
                    .Select(x => sync.Read<AssetFoldersModel>(x, log))
                    .ToList();

            writer.Paragraph($"{models.SelectMany(x => x.Paths).Distinct().Count()} asset folder(s).");

            return Task.CompletedTask;
        }

        public async Task ImportAsync(ISyncService sync, SyncOptions options, ISession session)
        {
            var models =
                GetFiles(sync.FileSystem)
                    .Select(x => sync.Read<AssetFoldersModel>(x, log))
                    .ToList();

            foreach (var model in models)
            {
                await log.DoSafeAsync("Importing folders", async () =>
                {
                    foreach (var path in model.Paths ?? Enumerable.Empty<string>())
                    {
                        await sync.Folders.GetIdAsync(path);
                    }
                });
            }
        }

        private static IEnumerable<IFile> GetFiles(IFileSystem fs)
        {
            foreach (var file in fs.GetFiles(new FilePath("assetFolders"), ".json"))
            {
                if (!file.Name.StartsWith("__", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

        public async Task GenerateSchemaAsync(ISyncService sync)
        {
            await sync.WriteJsonSchemaAsync<AssetFoldersModel>(new FilePath("assetFolders.json"));

            var sample = new AssetFoldersModel
            {
                Paths = new List<string>
                {
                    "images",
                    "documents",
                    "videos"
                }
            };

            await sync.WriteWithSchema(new FilePath("assetFolders", "__assetFolder.json"), sample, Ref);
        }
    }
}
