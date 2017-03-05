// ==========================================================================
//  IAuthenticator.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public interface IAuthenticator
    {
        Task<string> GetBearerTokenAsync();
    }
}