// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;

namespace Squidex.Identity.Model
{
    public static class Extensions
    {
        public static List<string> ToListByComma(this string value)
        {
            return value?
                .Split(',', ';')
                .Select(x => x.Trim())
                .ToList() ?? new List<string>();
        }
    }
}
