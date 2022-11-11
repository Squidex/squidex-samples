// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.FileSystem;
using Xunit;

namespace Squidex.CLI
{
    public class GitFileSystemTests
    {
        [Fact]
        public async Task Should_create_file_system()
        {
            using var sut = await FileSystems.CreateAsync("https://github.com/Squidex/templates.git");
        }

        [Fact]
        public async Task Should_open_file()
        {
            using var sut = await FileSystems.CreateAsync("https://github.com/Squidex/templates.git");

            var file = sut.GetFile("Readme.md");

            Assert.True(file.Exists);
            Assert.True(file.Size > 1000);
        }

        [Fact]
        public async Task Should_open_file_in_subfolder()
        {
            using var sut = await FileSystems.CreateAsync("https://github.com/Squidex/templates.git?folder=sample-profile");

            var file = sut.GetFile("schemas/skills.json");

            Assert.True(file.Exists);
            Assert.True(file.Size > 1000);
        }

        [Fact]
        public async Task Should_not_return_invalid_file()
        {
            using var sut = await FileSystems.CreateAsync("https://github.com/Squidex/templates.git?folder=sample-profile");

            var file = sut.GetFile("notfound.json");

            Assert.False(file.Exists);
        }
    }
}
