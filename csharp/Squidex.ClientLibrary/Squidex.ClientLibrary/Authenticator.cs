// ==========================================================================
//  Authenticator.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

// ReSharper disable InvertIf

namespace Squidex.ClientLibrary
{
    public class Authenticator : IAuthenticator
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly Uri serviceUrl;

        public Authenticator(Uri serviceUrl, string clientId, string clientSecret)
        {
            Guard.NotNull(serviceUrl, nameof(serviceUrl));
            Guard.NotNullOrEmpty(clientId, nameof(clientId));
            Guard.NotNullOrEmpty(clientSecret, nameof(clientSecret));

            this.serviceUrl = serviceUrl;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        public async Task<string> GetBearerTokenAsync()
        {
            var result = GetFromCache();

            if (result == null)
            {
                result = await GetBearerTokenFromServerAsync();

                SetToCache(result, DateTimeOffset.UtcNow.AddDays(50));
            }

            return result;
        }

        protected virtual void SetToCache(string result, DateTimeOffset expires)
        {
        }

        protected virtual string GetFromCache()
        {
            return null;
        }

        private async Task<string> GetBearerTokenFromServerAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"{serviceUrl}/identity-server/connect/token";

                var bodyString = $"grant_type=client_credentials&client_id={this.clientId}&client_secret={this.clientSecret}&scope=squidex-api";
                var bodyContent = new StringContent(bodyString, Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await httpClient.PostAsync(url, bodyContent);

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonToken = JToken.Parse(jsonString);

                return jsonToken["access_token"].ToString();
            }
        }
    }
}
