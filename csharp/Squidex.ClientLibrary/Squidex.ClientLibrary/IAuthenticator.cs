// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public interface IAuthenticator
    {
        Task<string> GetBearerTokenAsync();

        Task RemoveTokenAsync(string token);
    }
}