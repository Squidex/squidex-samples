// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.Identity.Model
{
    public sealed class UserLogin
    {
        public string LoginProvider { get; set; }

        public string DisplayName { get; set; }

        public string ProviderKey { get; set; }
    }
}