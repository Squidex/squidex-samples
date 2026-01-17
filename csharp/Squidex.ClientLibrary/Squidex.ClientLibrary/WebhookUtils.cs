// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Security.Cryptography;
using System.Text;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary;

/// <summary>
/// Provides helper methods for webhooks.
/// </summary>
public static class WebhookUtils
{
    /// <summary>
    /// Calculates the signature of the the request body and the secret.
    /// </summary>
    /// <param name="requestBody">The request body.</param>
    /// <param name="sharedSecret">The shared secret.</param>
    /// <returns>
    /// The calculated signature.
    /// </returns>
    public static string CalculateSignature(string requestBody, string sharedSecret)
    {
        Guard.NotNull(requestBody, nameof(requestBody));
        Guard.NotNull(sharedSecret, nameof(sharedSecret));

        return (requestBody + sharedSecret).ToSha256Base64();
    }

    private static string ToSha256Base64(this string value)
    {
        return ToSha256Base64(Encoding.UTF8.GetBytes(value));
    }

    private static string ToSha256Base64(this byte[] bytes)
    {
        using var sha = SHA256.Create();
        var bytesHash = sha.ComputeHash(bytes);

        var result = Convert.ToBase64String(bytesHash);

        return result;
    }
}
