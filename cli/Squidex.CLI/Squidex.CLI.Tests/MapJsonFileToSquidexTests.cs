// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.CLI.Commands;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Commands.Implementation.ImExport;
using Squidex.ClientLibrary;
using Xunit;

namespace Squidex.CLI.Tests
{
    public class MapJsonFileToSquidexTests
    {
        private readonly JsonTextReader jsonReader;

        public MapJsonFileToSquidexTests()
        {
            var data = new[]
            {
                new
                {
                    text = "Hello World",
                    boolean = true,
                    number = 1234,
                    array = new string[] { "Squidex", "CLI" },
                    obj = new { Squidex = "CLI" }
                }
            };

            var json = JsonConvert.SerializeObject(data);

            jsonReader = new JsonTextReader(new StringReader(json));
        }

        [Fact]
        public void Should_throw_exception_if_field_names_is_null()
        {
            Assert.Throws<SquidexException>(() => new Json2SquidexConverter(null));
        }

        [Fact]
        public void Should_throw_exception_if_field_names_is_empty()
        {
            Assert.Throws<SquidexException>(() => new Json2SquidexConverter(string.Empty));
        }

        [Fact]
        public void Should_read_string_to_invariant()
        {
            var sut = new Json2SquidexConverter("text");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["text"] = new Dictionary<string, JToken>
                {
                    ["iv"] = "Hello World"
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_boolean_to_invariant()
        {
            var sut = new Json2SquidexConverter("boolean");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["boolean"] = new Dictionary<string, JToken>
                {
                    ["iv"] = true
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_number_to_invariant()
        {
            var sut = new Json2SquidexConverter("number");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["number"] = new Dictionary<string, JToken>
                {
                    ["iv"] = 1234
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_string_to_localized()
        {
            var sut = new Json2SquidexConverter("text.de=text");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["text"] = new Dictionary<string, JToken>
                {
                    ["de"] = "Hello World"
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_array_to_invariant()
        {
            var sut = new Json2SquidexConverter("array");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["array"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JArray("Squidex", "CLI")
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_object_to_invariant()
        {
            var sut = new Json2SquidexConverter("obj");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["obj"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JObject(new JProperty("Squidex", "CLI"))
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_string_to_array()
        {
            var sut = new Json2SquidexConverter("json.iv.1=text");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["json"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JArray(null, "Hello World")
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_multiple_values_to_array()
        {
            var sut = new Json2SquidexConverter("json.iv.1=text,json.iv.0=number");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["json"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JArray(1234, "Hello World")
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_string_to_nested_array()
        {
            var sut = new Json2SquidexConverter("json.iv.1.0=text");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["json"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JArray(null, new JArray("Hello World"))
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_string_to_object()
        {
            var sut = new Json2SquidexConverter("json.iv.a=text");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["json"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JObject
                    {
                        ["a"] = "Hello World"
                    }
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_multiple_values_to_object()
        {
            var sut = new Json2SquidexConverter("json.iv.a=text,json.iv.b=number");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["json"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JObject
                    {
                        ["a"] = "Hello World",
                        ["b"] = 1234
                    }
                }
            };

            EqualJson(expected, actual);
        }

        [Fact]
        public void Should_read_string_to_nested_object()
        {
            var sut = new Json2SquidexConverter("json.iv.a0.a1=text");

            var actual = sut.ReadAll(jsonReader).First();

            var expected = new DummyData
            {
                ["json"] = new Dictionary<string, JToken>
                {
                    ["iv"] = new JObject
                    {
                        ["a0"] = new JObject
                        {
                            ["a1"] = "Hello World"
                        }
                    }
                }
            };

            EqualJson(expected, actual);
        }

        private void EqualJson(DummyData expected, DummyData actual)
        {
            Assert.Equal(JsonConvert.SerializeObject(expected, Formatting.Indented), JsonConvert.SerializeObject(actual, Formatting.Indented));
        }
    }
}
