// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Squidex.Identity.Model.Authentication
{
    public interface IAuthenticationSchemeStore
    {
        Task<List<AuthenticationSchemeData>> GetSchemesAsync();
    }
}
