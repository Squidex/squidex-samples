// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Squidex.ClientLibrary.Management
{
    /// <summary>
    /// A json converter that can handle inheritance.
    /// </summary>
    /// <seealso cref="JsonConverter" />
    public class JsonInheritanceConverter : JsonConverter
    {
        [ThreadStatic]
        private static bool isReading;

        [ThreadStatic]
        private static bool isWriting;

        /// <summary>
        /// Gets the name of the discriminator property.
        /// </summary>
        /// <value>
        /// The name of the discriminator property.
        /// </value>
        public string DiscriminatorName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInheritanceConverter"/> class.
        /// </summary>
        public JsonInheritanceConverter()
        {
            DiscriminatorName = "discriminator";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonInheritanceConverter"/> class with the name.
        /// of the discriminator property.
        /// </summary>
        /// <param name="discriminator">The name of the discriminator property.</param>
        public JsonInheritanceConverter(string discriminator)
        {
            DiscriminatorName = discriminator;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {
                isWriting = true;

                var jObject = JObject.FromObject(value, serializer);

                jObject.AddFirst(new JProperty(DiscriminatorName, GetSubtypeDiscriminator(value.GetType())));

                writer.WriteToken(jObject.CreateReader());
            }
            finally
            {
                isWriting = false;
            }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                if (isWriting)
                {
                    isWriting = false;
                    return false;
                }

                return true;
            }
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get
            {
                if (isReading)
                {
                    isReading = false;
                    return false;
                }

                return true;
            }
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = serializer.Deserialize<JObject>(reader);

            if (jObject == null)
            {
                return null;
            }

            var discriminator = jObject.GetValue(DiscriminatorName).Value<string>();

            var subtype = GetObjectSubtype(objectType, discriminator);

            if (!(serializer.ContractResolver.ResolveContract(subtype) is JsonObjectContract objectContract) || objectContract.Properties.All(p => p.PropertyName != DiscriminatorName))
            {
                jObject.Remove(DiscriminatorName);
            }

            try
            {
                isReading = true;
                return serializer.Deserialize(jObject.CreateReader(), subtype);
            }
            finally
            {
                isReading = false;
            }
        }

        /// <summary>
        /// Gets the discriminator value for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The discriminator value for the specified type.
        /// </returns>
        public string GetDiscriminatorValue(Type type)
        {
            return GetSubtypeDiscriminator(type);
        }

        private static Type GetObjectSubtype(Type objectType, string discriminator)
        {
            var attributes = objectType.GetTypeInfo().GetCustomAttributes<JsonInheritanceAttribute>(true);

            foreach (var attribute in attributes)
            {
                if (attribute.Key == discriminator)
                {
                    return attribute.Type;
                }
            }

            return objectType;
        }

        private static string GetSubtypeDiscriminator(Type objectType)
        {
            var attributes = objectType.GetTypeInfo().GetCustomAttributes<JsonInheritanceAttribute>(true);

            foreach (var attribute in attributes)
            {
                if (attribute.Type == objectType)
                {
                    return attribute.Key;
                }
            }

            return objectType.Name;
        }
    }
}
