// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public abstract class SquidexClientBase
    {
        protected string ApplicationName { get; }
        protected Uri ServiceUrl { get; }
        protected string SchemaName { get; }
        protected IAuthenticator Authenticator { get; }

        protected SquidexClientBase(Uri serviceUrl, string applicationName, string schemaName, IAuthenticator authenticator)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNull(authenticator, nameof(authenticator));
            Guard.NotNullOrEmpty(applicationName, nameof(applicationName));

            ServiceUrl = serviceUrl;
            SchemaName = schemaName;
            Authenticator = authenticator;
            ApplicationName = applicationName;
        }

        protected string BuildSquidexUri(SquidexUriKind uriKind, string path = "")
        {
            switch (uriKind)
            {
                case SquidexUriKind.Content:
                    return $"api/content/{ApplicationName}/{SchemaName}/{path}";
                case SquidexUriKind.Assets:
                    return $"api/assets/{path}";
                case SquidexUriKind.AppAssets:
                    return $"api/apps/{ApplicationName}/assets/{path}";
                default:
                    throw new NotSupportedException($"The uri kind '{uriKind}' is not supported.");
            }
        }

        protected async Task<HttpResponseMessage> RequestAsync(HttpMethod method, SquidexUriKind uriKind, string path = "", HttpContent content = null, QueryContext context = null)
        {
            var uri = new Uri(ServiceUrl, BuildSquidexUri(uriKind, path));

            var requestToken = await Authenticator.GetBearerTokenAsync();
            var request = BuildRequest(method, content, uri, requestToken);

            if (context != null)
            {
                if (context.IsFlatten)
                {
                    request.Headers.TryAddWithoutValidation("X-Flatten", "true");
                }

                if (context.Languages != null)
                {
                    var languages = string.Join(", ", context.Languages.Where(x => !string.IsNullOrWhiteSpace(x)));

                    if (!string.IsNullOrWhiteSpace(languages))
                    {
                        request.Headers.TryAddWithoutValidation("X-Languages", languages);
                    }
                }
            }

            var response = await SquidexHttpClient.Instance.SendAsync(request);

            await EnsureResponseIsValidAsync(response, requestToken);

            return response;
        }

        protected static HttpRequestMessage BuildRequest(HttpMethod method, HttpContent content, Uri uri, string requestToken)
        {
            var request = new HttpRequestMessage(method, uri);

            if (content != null)
            {
                request.Content = content;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", requestToken);

            return request;
        }

        protected async Task EnsureResponseIsValidAsync(HttpResponseMessage response, string token)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Authenticator.RemoveTokenAsync(token);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new SquidexException("The app, schema or entity does not exist.");
                }

                var message = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Squidex API failed with internal error.";
                }
                else
                {
                    message = $"Squidex Request failed: {message}";
                }

                throw new SquidexException(message);
            }
        }
    }
}
