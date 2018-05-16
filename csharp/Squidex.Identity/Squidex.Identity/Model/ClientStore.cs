// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class ClientStore : IClientStore
    {
        private readonly SquidexClient<SquidexClient, SquidexClientData> apiClient;

        public ClientStore(SquidexClientManager clientManager)
        {
            apiClient = clientManager.GetClient<SquidexClient, SquidexClientData>("clients");
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await apiClient.GetAsync(clientId);

            if (client == null)
            {
                return null;
            }

            return new Client
            {
                AllowedCorsOrigins = client.Data.AllowedCorsOrigins.ToListFromCommataSeparated(),
                AllowedGrantTypes = client.Data.AllowedGrantTypes.ToListFromCommataSeparated(),
                AllowedScopes = client.Data.AllowedScopes.ToListFromCommataSeparated(),
                AllowOfflineAccess = client.Data.AllowOfflineAccess,
                ClientId = clientId,
                ClientName = client.Data.ClientName,
                ClientSecrets = client.Data.ClientSecrets.ToSecretsListFromCommataSeparated(),
                ClientUri = client.Data.ClientUri,
                // LogoUri = client.Data.Logo,
                RedirectUris = client.Data.RedirectUris.ToListFromCommataSeparated(),
                RequireConsent = client.Data.RequireConsent,
                PostLogoutRedirectUris = client.Data.PostLogoutRedirectUris.ToListFromCommataSeparated()
            };
        }
    }
}
