// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.AspNetCore.Mvc;

namespace Integration.Localize.Controllers
{
    public partial class DefaultController : ControllerBase
    {
        public override Task<AuthGetResponse> AuthGET(
            CancellationToken cancellationToken = default)
        {
            var response = new AuthGetResponse
            {
                Type = AuthGetResponseType.ApiToken
            };

            return Task.FromResult(response);
        }

        public override async Task<IDictionary<string, string>> AuthPOST([FromBody] IDictionary<string, string>? body = null,
            CancellationToken cancellationToken = default)
        {
            var clientManager = BuildClientManager(body);

            // The permission is needed to get the schemas.
            await clientManager.CreateSchemasClient().GetSchemasAsync(clientManager.App,
                cancellationToken);

            // The permission is needed to get the languages.
            await clientManager.CreateAppsClient().GetLanguagesAsync(clientManager.App,
                cancellationToken);

            return body!;
        }

        public override Task<IDictionary<string, string>> Refresh(
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public override Task<IDictionary<string, string>> Response([FromBody] AuthResponseRequest? body = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
