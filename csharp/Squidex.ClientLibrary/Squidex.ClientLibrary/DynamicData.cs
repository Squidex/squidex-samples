// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Squidex.ClientLibrary
{
    [KeepCasing]
    public sealed class DynamicData : Dictionary<string, JObject>
    {
    }
}
