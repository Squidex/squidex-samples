// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Squidex.ClientLibrary;

/// <summary>
/// Annote your data object to keep the casing and to not convert it to camel case.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public sealed class KeepCasingAttribute : JsonContainerAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeepCasingAttribute"/> class.
    /// </summary>
    public KeepCasingAttribute()
    {
        NamingStrategyType = typeof(DefaultNamingStrategy);
    }
}
