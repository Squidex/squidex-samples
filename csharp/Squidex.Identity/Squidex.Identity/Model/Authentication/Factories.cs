// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;

namespace Squidex.Identity.Model.Authentication
{
    public static class Factories
    {
        public static TOptions OAuth<TOptions>(AuthenticationSchemeData data) where TOptions : OAuthOptions, new()
        {
            var result = new TOptions
            {
                ClientId = data.ClientId,
                ClientSecret = data.ClientSecret
            };

            if (data.Scopes != null)
            {
                foreach (var scope in data.Scopes)
                {
                    result.Scope.Add(scope);
                }
            }

            return result;
        }

        public static TwitterOptions Twitter(AuthenticationSchemeData data)
        {
            return new TwitterOptions
            {
                ConsumerKey = data.ClientSecret,
                ConsumerSecret = data.ClientSecret
            };
        }
    }
}
