// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using CoreTweet;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;

namespace Squidex.CLI.Commands
{
    public sealed partial class App
    {
        [Command("twitter", Description = "Manage twitter.")]
        [Subcommand]
        public sealed class Twitter
        {
            private readonly ILogger log;

            public Twitter(ILogger log)
            {
                this.log = log;
            }

            [Command("auth", Description = "Starts the authentication.")]
            public async Task Auth(AuthArguments arguments)
            {
                var session = await OAuth.AuthorizeAsync(arguments.ClientId, arguments.ClientSecret);

                await File.WriteAllTextAsync(".twitterToken", session.RequestToken);
                await File.WriteAllTextAsync(".twitterSecret", session.RequestTokenSecret);

                log.WriteLine($"Request Token:  {session.RequestToken}");
                log.WriteLine($"Request Secret: {session.RequestTokenSecret}");

                log.WriteLine();
                log.WriteLine($"Open the following url to get the pin: {session.AuthorizeUri}");
            }

            [Command("token", Description = "Create an access token and secret.")]
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

                log.WriteLine($"Access Token:  {tokens.AccessToken}");
                log.WriteLine($"Access Secret: {tokens.AccessTokenSecret}");
            }

            private string ReadToken(string fromArgs, string file, string parameter)
            {
                var requestToken = fromArgs;

                if (string.IsNullOrWhiteSpace(requestToken))
                {
                    if (File.Exists(".twitterToken"))
                    {
                        requestToken = File.ReadAllText(file);

                        log.WriteLine($"Using {parameter} {requestToken} from last command.");
                        log.WriteLine();
                    }
                }

                if (string.IsNullOrWhiteSpace(requestToken))
                {
                    log.WriteLine($"{parameter} not defined or found from last command.");
                }

                return requestToken;
            }

            public sealed class AuthArguments : AppArguments
            {
                [Option("clientId")]
                public string ClientId { get; set; } = "QZhb3HQcGCvE6G8yNNP9ksNet";

                [Option("clientSecret")]
                public string ClientSecret { get; set; } = "Pdu9wdN72T33KJRFdFy1w4urBKDRzIyuKpc0OItQC2E616DuZD";

                public sealed class Validator : AbstractValidator<AuthArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.ClientId).NotEmpty();
                        RuleFor(x => x.ClientSecret).NotEmpty();
                    }
                }
            }

            public sealed class TokenArguments : AppArguments
            {
                [Operand("pin", Description = "The pin from the auth request.")]
                public string PinCode { get; set; }

                [Option("clientId")]
                public string ClientId { get; set; } = "QZhb3HQcGCvE6G8yNNP9ksNet";

                [Option("clientSecret")]
                public string ClientSecret { get; set; } = "Pdu9wdN72T33KJRFdFy1w4urBKDRzIyuKpc0OItQC2E616DuZD";

                [Option("token")]
                public string RequestToken { get; set; }

                [Option("secret")]
                public string RequestTokenSecret { get; set; }

                public sealed class Validator : AbstractValidator<TokenArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.PinCode).NotEmpty();
                    }
                }
            }
        }
    }
}
