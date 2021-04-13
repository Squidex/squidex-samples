// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.Utils;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class SerializationTests
    {
        public sealed class MyClass<T>
        {
            [JsonConverter(typeof(InvariantConverter))]
            public T Value { get; set; }
        }

        public sealed class MyCamelClass<T>
        {
            public T Value { get; set; }
        }

        [KeepCasing]
        public sealed class MyPascalClass<T>
        {
            public T Value { get; set; }
        }

        private readonly JsonSerializerSettings settings = new JsonSerializerSettings();

        public SerializationTests()
        {
            settings.Converters.Add(new InvariantConverter());
        }

        [Fact]
        public void Should_serialize_datetime_to_iso8601()
        {
            var utcTime = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Utc);

            var source = new MyClass<DateTime>
            {
                Value = utcTime
            };

            var serialized = source.ToJson();

            Assert.Contains("2012-11-10T09:08:07Z", serialized);
        }

        [Fact]
        public void Should_serialize_local_datetime_to_iso8601_utc()
        {
            var utcTime = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Utc);

            var source = new MyClass<DateTime>
            {
                Value = utcTime.ToLocalTime()
            };

            var serialized = source.ToJson();

            Assert.Contains("2012-11-10T09:08:07Z", serialized);
        }

        [Fact]
        public void Should_serialize_datetimeoffset_to_iso8601()
        {
            var utcTime = new DateTimeOffset(2012, 11, 10, 9, 8, 7, TimeSpan.Zero);

            var source = new MyClass<DateTimeOffset>
            {
                Value = utcTime
            };

            var serialized = source.ToJson();

            Assert.Contains("2012-11-10T09:08:07Z", serialized);
        }

        [Fact]
        public void Should_serialize_local_datetimeoffset_to_iso8601_utc()
        {
            var utcTime = new DateTimeOffset(2012, 11, 10, 9, 8, 7, TimeSpan.Zero);

            var source = new MyClass<DateTimeOffset>
            {
                Value = utcTime.ToLocalTime()
            };

            var serialized = source.ToJson();

            Assert.Contains("2012-11-10T09:08:07Z", serialized);
        }

        [Fact]
        public void Should_serialize_local_datetime_to_iso8601_utc_with_ms()
        {
            var utcTime = new DateTime(634292263478068039);

            var source = new MyClass<DateTime>
            {
                Value = utcTime.ToLocalTime()
            };

            var serialized = source.ToJson();

            Assert.Contains("2010-12-29T13:32:27Z", serialized);
        }

        [Fact]
        public void Should_serialize_false()
        {
            var source = new BulkUpdate
            {
                DoNotScript = false
            };

            var serialized = source.ToJson();

            Assert.Contains("\"doNotScript\": false", serialized);
        }

        [Fact]
        public void Should_serialize_type()
        {
            var source = new BulkUpdateJob
            {
                Type = BulkUpdateType.ChangeStatus
            };

            var serialized = source.ToJson();

            Assert.Contains("\"type\": \"ChangeStatus\"", serialized);
        }

        [Fact]
        public void Should_serialize_invariant()
        {
            var source = new MyClass<string>
            {
                Value = "hello"
            };

            var serialized = source.ToJson();

            Assert.Contains("\"iv\": \"hello\"", serialized);
        }

        [Fact]
        public void Should_serialize_dynamic_properties_with_original_casing()
        {
            var source = new DynamicData
            {
                ["Property1"] = new JObject()
            };

            var serialized = source.ToJson();

            Assert.Contains("\"Property1\": {}", serialized);
        }

        [Fact]
        public void Should_deserialize_invariant()
        {
            var json = "{ 'value': { 'iv': 'hello'} }";

            var res = JsonConvert.DeserializeObject<MyClass<string>>(json, settings);

            Assert.Equal("hello", res.Value);
        }

        [Fact]
        public void Should_deserialize_invariant_null_value()
        {
            var json = "{ 'value': null }";

            var res = JsonConvert.DeserializeObject<MyClass<string>>(json, settings);

            Assert.Null(res.Value);
        }

        [Fact]
        public void Should_deserialize_invariant_empty_value()
        {
            var json = "{ 'value': {} }";

            var res = JsonConvert.DeserializeObject<MyClass<string>>(json, settings);

            Assert.Null(res.Value);
        }

        [Fact]
        public void Should_serialize_with_camel_case()
        {
            var source = new MyCamelClass<string>
            {
                Value = "hello"
            };

            var serialized = source.ToJson();

            Assert.Contains("\"value\": \"hello\"", serialized);
        }

        [Fact]
        public void Should_serialize_with_pascal_case()
        {
            var source = new MyPascalClass<string>
            {
                Value = "hello"
            };

            var serialized = source.ToJson();

            Assert.Contains("\"Value\": \"hello\"", serialized);
        }
    }
}
