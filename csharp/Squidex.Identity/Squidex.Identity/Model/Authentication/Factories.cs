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
            return new TOptions
            {
                ClientId = data.ClientId,
                ClientSecret = data.ClientSecret
            };
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
