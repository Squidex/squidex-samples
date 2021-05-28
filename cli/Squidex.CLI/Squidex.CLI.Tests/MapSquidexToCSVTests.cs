// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.ImExport;
using Squidex.ClientLibrary;
using Xunit;

namespace Squidex.CLI.Tests
{
    public class MapSquidexToCSVTests
    {
        private readonly DynamicContent content;

        public MapSquidexToCSVTests()
        {
            content = new DynamicContent
            {
                Created = DateTimeOffset.Now
            };

            content.Data["name"] = new JObject
            {
                ["iv"] = "Hello World"
            };

            content.Data["text"] = new JObject
            {
                ["iv"] = "Hello World"
            };

            content.Data["localizedText"] = new JObject
            {
                ["en"] = "Hello World"
            };

            content.Data["multilineText"] = new JObject
            {
                ["iv"] = "Hello\nWorld"
            };

            content.Data["user"] = new JObject
            {
                ["iv"] = new JObject(new JProperty("name", "Squidex"))
            };

            content.Data["products"] = new JObject
            {
                ["iv"] = new JArray("Squidex", "CLI")
            };
        }

        [Fact]
        public void Should_throw_exception_if_field_names_is_null()
        {
            Assert.Throws<CLIException>(() => new Squidex2CsvConverter(null));
        }

        [Fact]
        public void Should_throw_exception_if_field_names_is_empty()
        {
            Assert.Throws<CLIException>(() => new Squidex2CsvConverter(string.Empty));
        }

        [Fact]
        public void Should_use_direct_field_names()
        {
            var sut = new Squidex2CsvConverter("created,lastModified,data.name.iv");

            Assert.Equal(new[] { "created", "lastModified", "data.name.iv" }, sut.FieldNames);
        }

        [Fact]
        public void Should_use_alias_field_names()
        {
            var sut = new Squidex2CsvConverter("created,name=data.name.iv");

            Assert.Equal(new[] { "created", "name" }, sut.FieldNames);
        }

        [Fact]
        public void Should_extract_value_from_property()
        {
            var sut = new Squidex2CsvConverter("created");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { content.Created }, values);
        }

        [Fact]
        public void Should_return_invalid_if_property_not_found()
        {
            var sut = new Squidex2CsvConverter("invalid");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "<INVALID>" }, values);
        }

        [Fact]
        public void Should_extract_value_from_invariant_data()
        {
            var sut = new Squidex2CsvConverter("data.text");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "Hello World" }, values);
        }

        [Fact]
        public void Should_extract_value_from_localized_data()
        {
            var sut = new Squidex2CsvConverter("data.localizedText.en");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "Hello World" }, values);
        }

        [Fact]
        public void Should_extract_value_from_invariant_data_and_escape()
        {
            var sut = new Squidex2CsvConverter("data.multilineText");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "Hello\\nWorld" }, values);
        }

        [Fact]
        public void Should_return_invalid_if_field_not_found()
        {
            var sut = new Squidex2CsvConverter("data.invalid");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "<INVALID>" }, values);
        }

        [Fact]
        public void Should_extract_value_from_nested_object()
        {
            var sut = new Squidex2CsvConverter("data.user.iv.name");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "Squidex" }, values);
        }

        [Fact]
        public void Should_return_invalid_if_property_in_nested_object_not_found()
        {
            var sut = new Squidex2CsvConverter("data.invalid");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "<INVALID>" }, values);
        }

        [Fact]
        public void Should_extract_value_from_nested_array()
        {
            var sut = new Squidex2CsvConverter("data.products.iv.1");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "CLI" }, values);
        }

        [Fact]
        public void Should_return_invalid_if_index_out_of_range()
        {
            var sut = new Squidex2CsvConverter("data.products.iv.3");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "<INVALID>" }, values);
        }

        [Fact]
        public void Should_extract_value_as_json()
        {
            var sut = new Squidex2CsvConverter("data.products");

            var values = sut.GetValues(content).ToArray();

            Assert.Equal(new object[] { "[\"Squidex\",\"CLI\"]" }, values);
        }
    }
}
