// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation.TestData;
using Xunit;

namespace Squidex.CLI;

public class LoremIpsumTests
{
    [Fact]
    public void Should_generate_single_character()
    {
        var result = LoremIpsum.Text(1, false);

        Assert.Equal("l", result);
    }

    [Fact]
    public void Should_generate_html_text()
    {
        for (var i = 0; i < 5000; i++)
        {
            var result = LoremIpsum.Text(i, true);

            Assert.NotNull(result);
        }
    }

    [Fact]
    public void Should_generate_text()
    {
        for (var i = 0; i < 5000; i++)
        {
            var result = LoremIpsum.Text(i, false);

            Assert.NotNull(result);
        }
    }
}
