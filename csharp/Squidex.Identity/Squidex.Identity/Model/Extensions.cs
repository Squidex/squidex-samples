// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;

namespace Squidex.Identity.Model
{
    public static class Extensions
    {
        public static List<string> ToListFromCommataSeparated(this string value)
        {
            return value?
                .Split(',', ';')
                .Select(x => x.Trim())
                .ToList() ?? new List<string>();
        }

        public static List<Secret> ToSecretsListFromCommataSeparated(this string value)
        {
            return value.ToListFromCommataSeparated().Select(x => new Secret(x.Sha256())).ToList();
        }
    }
}
