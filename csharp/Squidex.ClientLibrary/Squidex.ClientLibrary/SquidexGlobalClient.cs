// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Transfer;

namespace Squidex.ClientLibrary
{
    public class SquidexGlobalClient : SquidexClientBase
    {
        public SquidexGlobalClient(string applicationName, string schemaName, HttpClient httpClient)
            : base(applicationName, httpClient)
        {
        }

        public async Task<List<GetAllUsersResponse>> GetAllUsers()

        {
            var request = await RequestAsync(HttpMethod.Get, "users");
            var response = await request.Content.ReadAsJsonAsync<List<GetAllUsersResponse>>();

            return response;
        }

        public async Task<CreateAppResponse> CreateApp(CreateAppRequest data)
        {
            var request = await RequestAsync(HttpMethod.Post, "apps", data.ToContent());
            var response = await request.Content.ReadAsJsonAsync<CreateAppResponse>();

            return response;
        }

        public async Task<CreateClientResponse> CreateClientCredentials(CreateClientRequest data)
        {
            var request = await RequestAsync(HttpMethod.Post, $"apps/{this.ApplicationName}/clients", data.ToContent());
            var response = await request.Content.ReadAsJsonAsync<CreateClientResponse>();

            return response;
        }

        public async Task AddContributor()
        {
            var user = await this.GetAllUsers();
            var data = new AddContributorRequest();
            data.ContributorId = user.Last().Id;
            data.Role = "Editor";
            var request = await RequestAsync(HttpMethod.Post, "apps/check-app/contributors", data.ToContent());
            var response = await request.Content.ReadAsStringAsync();
        }
    }
}