// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// A dynamic data object.
    /// </summary>
    /// <remarks>
    /// Use this type when you have a dynamic structure for your content or if you query content items across many schemas.
    /// </remarks>
    [KeepCasing]
    public sealed class DynamicData : Dictionary<string, JToken>
    {
    }
}
