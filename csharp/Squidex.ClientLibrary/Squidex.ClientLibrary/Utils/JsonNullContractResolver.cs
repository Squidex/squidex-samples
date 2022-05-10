// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Squidex.ClientLibrary.Utils
{
    internal class JsonNullContractResolver : CamelCasePropertyNamesContractResolver
    {
        /*
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (contract.UnderlyingType.IsGenericType && contract.UnderlyingType.GetGenericTypeDefinition() == typeof(JsonNull<>))
            {
                var converterType = typeof(JsonNullConverter<>).MakeGenericType(contract.UnderlyingType.GetGenericArguments()[0]);
                var converterObj = Activator.CreateInstance(converterType) as JsonConverter;

                contract.Converter = converterObj;
            }

            return contract;
        }
        */

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
}
