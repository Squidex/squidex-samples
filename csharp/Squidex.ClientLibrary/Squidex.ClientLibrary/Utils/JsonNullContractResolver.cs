// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Squidex.ClientLibrary.Utils;

/// <summary>
/// a custom contract resolver that registeres the necessary converters.
/// </summary>
public class JsonNullContractResolver : CamelCasePropertyNamesContractResolver
{
    /// <inheritdoc/>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        var objectType = (member as PropertyInfo)?.PropertyType;

        if (objectType?.IsGenericType == true && objectType.GetGenericTypeDefinition() == typeof(JsonNull<>))
        {
            var baseType = typeof(JsonNullConverter<>);

            if (property.Converter is InvariantConverter)
            {
                baseType = typeof(JsonNullInvariantConverter<>);
            }
            else if (property.Converter is InvariantWriteConverter)
            {
                baseType = typeof(JsonNullInvariantWriteConverter<>);
            }

            var converterType = baseType.MakeGenericType(objectType.GetGenericArguments()[0]);
            var converterObj = Activator.CreateInstance(converterType) as JsonConverter;

            property.Converter = converterObj;
        }

        return property;
    }

    /// <inheritdoc/>
    protected override JsonConverter? ResolveContractConverter(Type objectType)
    {
        if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(JsonNull<>))
        {
            var converterType = typeof(JsonNullConverter<>).MakeGenericType(objectType.GetGenericArguments()[0]);
            var converterObj = Activator.CreateInstance(converterType) as JsonConverter;

            return converterObj;
        }

        return base.ResolveContractConverter(objectType);
    }
}
