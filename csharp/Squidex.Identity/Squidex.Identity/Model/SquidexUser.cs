// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class SquidexUser : SquidexEntityBase<SquidexUserData>
    {
        public static SquidexUser Create(string email)
        {
            var result = new SquidexUser();

            result.Data.UserName = email;
            result.Data.Email = email;

            return result;
        }
    }
}