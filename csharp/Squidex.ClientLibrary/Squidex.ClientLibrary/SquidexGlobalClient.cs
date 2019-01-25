// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Dto;

namespace Squidex.ClientLibrary
{
    public class SquidexGlobalClient : SquidexClientBase
    {
        public SquidexGlobalClient(Uri serviceUrl, string applicationName, string schemaName, IAuthenticator authenticator)
            : base(serviceUrl, applicationName, schemaName, authenticator)
        {
        }

        public async Task<List<GetAllUsersResponse>> GetAllUsers()
        {
            var request = await RequestAsync(HttpMethod.Get, "api/users");
            var response = await request.Content.ReadAsJsonAsync<List<GetAllUsersResponse>>();

            return response;
        }
    }
}
