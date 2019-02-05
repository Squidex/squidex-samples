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
                Url = new Uri("http://localhost:5000/api/apps/aa/backups/547eb007-e702-43a0-a71a-a24faa4efa79"), Name = "app2"
            });
        }
    }
}
