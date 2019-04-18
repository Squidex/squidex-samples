// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Attributes;
using CoreTweet;
using FluentValidation;
using FluentValidation.Attributes;

namespace Squidex.CLI.Commands
{
    public sealed partial class App
    {
        [ApplicationMetadata(Name = "twitter", Description = "Manage twitter.")]
        [SubCommand]
        public sealed class Twitter
        {
            [ApplicationMetadata(Name = "auth", Description = "Starts the authentication.")]
            public async Task Auth(AuthArguments arguments)
            {
                var session = await OAuth.AuthorizeAsync(arguments.ClientId, arguments.ClientSecret);

                File.WriteAllText(".twitterToken", session.RequestToken);
                File.WriteAllText(".twitterSecret", session.RequestTokenSecret);

                Console.WriteLine($"Request Token:  {session.RequestToken}");
                Console.WriteLine($"Request Secret: {session.RequestTokenSecret}");

                Console.WriteLine();
                Console.WriteLine($"Open the following url to get the pin: {session.AuthorizeUri}");
            }

            [ApplicationMetadata(Name = "token", Description = "Create an access token and secret.")]
            public async Task Token(TokenArguments arguments)
            {
                var requestToken = ReadToken(arguments.RequestToken, ".twitterToken", nameof(arguments.RequestToken));

                var requestTokenSecret = ReadToken(arguments.RequestTokenSecret, ".twitterSecret", nameof(arguments.RequestTokenSecret));

                var session = new OAuth.OAuthSession
                {
                    ConsumerKey = arguments.ClientId,
                    ConsumerSecret = arguments.ClientSecret,
                    RequestToken = requestToken,
                    RequestTokenSecret = requestTokenSecret
                };

                var tokens = await session.GetTokensAsync(arguments.PinCode);

                Console.WriteLine($"Access Token:  {tokens.AccessToken}");
                Console.WriteLine($"Access Secret: {tokens.AccessTokenSecret}");
            }

            private static string ReadToken(string fromArgs, string file, string parameter)
            {
                var requestToken = fromArgs;

                if (string.IsNullOrWhiteSpace(requestToken))
                {
                    if (File.Exists(".twitterToken"))
                    {
                        requestToken = File.ReadAllText(file);

                        Console.WriteLine($"Using {parameter} {requestToken} from last command.");
                        Console.WriteLine();
                    }
                }

                if (string.IsNullOrWhiteSpace(requestToken))
                {
                    Console.WriteLine($"{parameter} not defined or found from last command.");
                }

                return requestToken;
            }

            [Validator(typeof(AuthArgumentsValidator))]
            public sealed class AuthArguments : IArgumentModel
            {
                [Option(ShortName ="clientId")]
                public string ClientId { get; set; } = "QZhb3HQcGCvE6G8yNNP9ksNet";

                [Option(ShortName = "clientSecret")]
                public string ClientSecret { get; set; } = "Pdu9wdN72T33KJRFdFy1w4urBKDRzIyuKpc0OItQC2E616DuZD";

                public sealed class AuthArgumentsValidator : AbstractValidator<AuthArguments>
                {
                    public AuthArgumentsValidator()
                    {
                        RuleFor(x => x.ClientId).NotEmpty();
                        RuleFor(x => x.ClientSecret).NotEmpty();
                    }
                }
            }

            [Validator(typeof(TokenArgumentsValidator))]
            public sealed class TokenArguments : IArgumentModel
            {
                [Argument(Name = "pin", Description = "The pin from the auth request.")]
                public string PinCode { get; set; }

                [Option(ShortName = "clientId")]
                public string ClientId { get; set; } = "QZhb3HQcGCvE6G8yNNP9ksNet";

                [Option(ShortName = "clientSecret")]
                public string ClientSecret { get; set; } = "Pdu9wdN72T33KJRFdFy1w4urBKDRzIyuKpc0OItQC2E616DuZD";

                [Option(ShortName = "token")]
                public string RequestToken { get; set; }

                [Option(ShortName = "secret")]
                public string RequestTokenSecret { get; set; }

                public sealed class TokenArgumentsValidator : AbstractValidator<TokenArguments>
                {
                    public TokenArgumentsValidator()
                    {
                        RuleFor(x => x.PinCode).NotEmpty();
                    }
                }
            }
        }
    }
}
