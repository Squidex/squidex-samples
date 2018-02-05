// ==========================================================================
//  SquidexException.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Net.Http;

namespace Squidex.ClientLibrary
{
    internal static class SquidexHttpClient
    {
        public static readonly HttpClient Instance = new HttpClient();
    }
}
