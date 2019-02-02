// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class RestoreTests
    {
        [Fact]
        public async Task Should_invoke_restore()
        {
            var clientManager = new SquidexClientManager("http://localhost:5000", "noapp", "test", "oy6kRrOl00PWsLuVuhCALjsTV9yTv18LlTEbVvTHnyM=");

            var client = clientManager.CreateBackupsClient();

            await client.PostRestoreAsync(new RestoreRequest
            {
                Url = new Uri("http://invalid")
            });
        }
    }
}
