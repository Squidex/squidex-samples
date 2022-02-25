// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Handles authentication tokens.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Gets the JWT bearer token.
        /// </summary>
        /// <returns>
        /// The JWT bearer token.
        /// </returns>
        Task<string> GetBearerTokenAsync();

        /// <summary>
        /// Removes a token when it has been expired or invalidated.
        /// </summary>
        /// <param name="token">The token to remove.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        Task RemoveTokenAsync(string token);
    }
}