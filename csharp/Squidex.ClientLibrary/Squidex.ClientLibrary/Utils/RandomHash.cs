// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Security.Cryptography;
using System.Text;

namespace Squidex.ClientLibrary.Utils
{
    public static class RandomHash
    {
        public static string New()
        {
            return Guid.NewGuid().ToString().Sha256Base64().Replace("+", "x").Sha256Base64();
        }

        public static string Sha256Base64(this string value)
        {
            using (var sha = SHA256.Create())
            {
                var bytesValue = Encoding.UTF8.GetBytes(value);
                var bytesHash = sha.ComputeHash(bytesValue);

                var result = Convert.ToBase64String(bytesHash);

                return result;
            }
        }
    }
}
