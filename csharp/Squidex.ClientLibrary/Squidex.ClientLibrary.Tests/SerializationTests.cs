// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;
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
        public void Should_serialize_invariant()
        {
            var source = new MyClass<string>
            {
                Value = "hello"
            };

            var serialized = source.ToJson();

            Assert.Contains("\"iv\": \"hello\"", serialized);
        }
    }
}
