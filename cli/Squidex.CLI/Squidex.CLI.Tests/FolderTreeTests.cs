// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using FakeItEasy;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.Sync;
using Squidex.ClientLibrary.Management;
using Xunit;

namespace Squidex.CLI.Tests
{
    public class FolderTreeTests
    {
        private static readonly string RootId = Guid.Empty.ToString();
        private readonly ISession session = A.Fake<ISession>();
        private readonly IAssetsClient assets = A.Fake<IAssetsClient>();
        private readonly FolderTree sut;

        public FolderTreeTests()
        {
            A.CallTo(() => session.Assets)
                .Returns(assets);

            A.CallTo(() => session.App)
                .Returns("my-app");

            sut = new FolderTree(session);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("/")]
        [InlineData(".")]
        [InlineData("./")]
        [InlineData("\\")]
        public async Task Should_provide_null_if_for_root_path(string path)
        {
            var id = await sut.GetIdAsync(path);

            Assert.Null(id);
        }

        [Fact]
        public async Task Should_query_for_path_once_for_each_subtree()
        {
            // * folder1
            // * folder2
            var folder1 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder1"
            };

            var folder2 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder2"
            };

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", folder1.Id, A<AssetFolderScope>._, A<CancellationToken>._))
                .Returns(new AssetFoldersDto
                {
                    Items = new List<AssetFolderDto>(),
                    Path = new List<AssetFolderDto>
                    {
                        folder1
                    }
                });

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", folder2.Id, A<AssetFolderScope>._, A<CancellationToken>._))
                .Returns(new AssetFoldersDto
                {
                    Items = new List<AssetFolderDto>(),
                    Path = new List<AssetFolderDto>
                    {
                        folder2
                    }
                });

            Assert.Equal("folder1", await sut.GetPathAsync(folder1.Id));
            Assert.Equal("folder2", await sut.GetPathAsync(folder2.Id));

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", A<string>._, A<AssetFolderScope>._, A<CancellationToken>._))
                .MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task Should_not_query_for_path_again_if_child_already_queried()
        {
            // * folder1
            //   * folder2
            var folder1 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder1"
            };

            var folder2 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder2"
            };

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", folder2.Id, A<AssetFolderScope>._, A<CancellationToken>._))
                .Returns(new AssetFoldersDto
                {
                    Items = new List<AssetFolderDto>(),
                    Path = new List<AssetFolderDto>
                    {
                        folder1,
                        folder2
                    }
                });

            Assert.Equal("folder1/folder2", await sut.GetPathAsync(folder2.Id));
            Assert.Equal("folder1", await sut.GetPathAsync(folder1.Id));

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", A<string>._, A<AssetFolderScope>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_not_query_for_path_again_if_parent_already_queried()
        {
            // * folder1
            //   * folder2
            var folder1 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder1"
            };

            var folder2 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder2"
            };

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", folder1.Id, A<AssetFolderScope>._, A<CancellationToken>._))
                .Returns(new AssetFoldersDto
                {
                    Items = new List<AssetFolderDto>
                    {
                        folder2
                    },
                    Path = new List<AssetFolderDto>
                    {
                        folder1
                    }
                });

            Assert.Equal("folder1", await sut.GetPathAsync(folder1.Id));
            Assert.Equal("folder1/folder2", await sut.GetPathAsync(folder2.Id));

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", A<string>._, A<AssetFolderScope>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_query_for_id_once_for_each_tree_item()
        {
            // * folder1
            // * folder2
            var folder1 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder1"
            };

            var folder2 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder2"
            };

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", RootId, A<AssetFolderScope>._, A<CancellationToken>._))
                .Returns(new AssetFoldersDto
                {
                    Items = new List<AssetFolderDto>
                    {
                        folder1,
                        folder2
                    },
                    Path = new List<AssetFolderDto>()
                });

            Assert.Equal(folder1.Id, await sut.GetIdAsync("folder1"));
            Assert.Equal(folder2.Id, await sut.GetIdAsync("folder2"));

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", A<string>._, A<AssetFolderScope>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_not_query_for_id_again_if_parent_already_queried()
        {
            // * folder1
            //   * folder2
            var folder1 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder1"
            };

            var folder2 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder2"
            };

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", folder1.Id, A<AssetFolderScope>._, A<CancellationToken>._))
                .Returns(new AssetFoldersDto
                {
                    Items = new List<AssetFolderDto>
                    {
                        folder2
                    },
                    Path = new List<AssetFolderDto>
                    {
                        folder1
                    }
                });

            Assert.Equal("folder1", await sut.GetPathAsync(folder1.Id));
            Assert.Equal("folder1/folder2", await sut.GetPathAsync(folder2.Id));

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", A<string>._, A<AssetFolderScope>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_query_for_id_once_and_create_new_folder()
        {
            // * folder1
            // * folder2
            var folder1 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder1"
            };

            var folder2 = new AssetFolderDto
            {
                Id = Guid.NewGuid().ToString(),
                FolderName = "folder2"
            };

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", RootId, A<AssetFolderScope>._, A<CancellationToken>._))
                .Returns(new AssetFoldersDto
                {
                    Items = new List<AssetFolderDto>
                    {
                        folder1
                    },
                    Path = new List<AssetFolderDto>()
                });

            A.CallTo(() => assets.PostAssetFolderAsync("my-app",
                    A<CreateAssetFolderDto>.That.Matches(x => x.FolderName == "folder2" && x.ParentId == RootId),
                    A<CancellationToken>._))
                .Returns(folder2);

            Assert.Equal(folder1.Id, await sut.GetIdAsync("folder1"));
            Assert.Equal(folder2.Id, await sut.GetIdAsync("folder2"));

            A.CallTo(() => assets.GetAssetFoldersAsync("my-app", A<string>._, A<AssetFolderScope>._, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
