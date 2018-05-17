// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;

namespace Squidex.Identity.Model
{
    public sealed class UserEntity : SquidexEntityBase<UserData>
    {
        public static UserEntity Create(string email)
        {
            var result = new UserEntity();

            result.Data.UserName = email;
            result.Data.Email = email;

            return result;
        }
    }
}