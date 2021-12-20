// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Squidex.CLI.Commands.Implementation.TestData;
using Squidex.ClientLibrary.Management;
using Xunit;

namespace Squidex.CLI.Tests
{
    public class TestDataGeneratorTests
    {
        [Fact]
        public void Should_generate_datetime_between_min_and_max()
        {
            var minValue = DateTime.UtcNow.AddDays(12);
            var maxValue = minValue.AddDays(10);

            var values =
                CreateManyScalars(
                    new DateTimeFieldPropertiesDto
                    {
                        Editor = DateTimeFieldEditor.DateTime,
                        MinValue = minValue,
                        MaxValue = maxValue
                    });

            var dates = values.Cast<DateTime>();

            var min = dates.Min();
            var max = dates.Max();

            Assert.True(min >= minValue.Date);
            Assert.True(max <= maxValue);

            Assert.True(dates.All(x => x.Kind == DateTimeKind.Utc));
        }

        [Fact]
        public void Should_generate_datetime_before_max()
        {
            var minValue = DateTime.UtcNow.AddDays(-10);
            var maxValue = minValue.AddDays(30);

            var values =
                CreateManyScalars(
                    new DateTimeFieldPropertiesDto
                    {
                        Editor = DateTimeFieldEditor.DateTime,
                        MinValue = null,
                        MaxValue = maxValue
                    });

            var dates = values.Cast<DateTime>();

            var min = dates.Min();
            var max = dates.Max();

            Assert.True(min >= minValue.Date);
            Assert.True(max <= maxValue);

            Assert.True(dates.All(x => x.Kind == DateTimeKind.Utc));
        }

        [Fact]
        public void Should_generate_datetime_after_min()
        {
            var minValue = DateTime.UtcNow.AddDays(-10);
            var maxValue = minValue.AddDays(30);

            var values =
                CreateManyScalars(
                    new DateTimeFieldPropertiesDto
                    {
                        Editor = DateTimeFieldEditor.DateTime,
                        MinValue = minValue,
                        MaxValue = null
                    });

            var dates = values.Cast<DateTime>();

            var min = dates.Min();
            var max = dates.Max();

            Assert.True(min >= minValue.Date);
            Assert.True(max <= maxValue);

            Assert.True(dates.All(x => x.Kind == DateTimeKind.Utc));
        }

        [Fact]
        public void Should_generate_datetime_with_default_range()
        {
            var minValue = DateTime.UtcNow;
            var maxValue = minValue.AddDays(30);

            var values =
                CreateManyScalars(
                    new DateTimeFieldPropertiesDto
                    {
                        Editor = DateTimeFieldEditor.DateTime
                    });

            var dates = values.Cast<DateTime>();

            var min = dates.Min();
            var max = dates.Max();

            Assert.True(min >= minValue.Date);
            Assert.True(max <= maxValue);

            Assert.True(dates.All(x => x.Kind == DateTimeKind.Utc));
        }

        [Fact]
        public void Should_generate_date_with_default_range()
        {
            var minValue = DateTime.UtcNow;
            var maxValue = minValue.AddDays(30);

            var values =
                CreateManyScalars(
                    new DateTimeFieldPropertiesDto());

            var dates = values.Cast<DateTime>();

            var min = dates.Min();
            var max = dates.Max();

            Assert.True(min >= minValue.Date);
            Assert.True(max <= maxValue.Date);

            Assert.True(dates.All(x => x.TimeOfDay == TimeSpan.Zero && x.Kind == DateTimeKind.Utc));
        }

        [Fact]
        public void Should_generate_booleans_from_required_field()
        {
            var values =
                CreateManyScalars(
                    new BooleanFieldPropertiesDto
                    {
                        IsRequired = true
                    });

            Assert.Equal(2, values.Count);
            Assert.Contains(true, values);
            Assert.Contains(false, values);
        }

        [Fact]
        public void Should_generate_booleans()
        {
            var values =
                CreateManyScalars(
                    new BooleanFieldPropertiesDto());

            Assert.Equal(3, values.Count);
            Assert.Contains(null, values);
            Assert.Contains(true, values);
            Assert.Contains(false, values);
        }

        [Fact]
        public void Should_generate_numbers_from_allowed_values()
        {
            var values =
                CreateManyScalars(
                    new NumberFieldPropertiesDto
                    {
                        AllowedValues = new List<double> { 13, 27, 42 }
                    });

            Assert.Equal(3, values.Count);
            Assert.Contains(13.0, values);
            Assert.Contains(27.0, values);
            Assert.Contains(42.0, values);
        }

        [Fact]
        public void Should_generate_numbers_between_min_and_max()
        {
            var values =
                CreateManyScalars(
                    new NumberFieldPropertiesDto
                    {
                        MinValue = 400,
                        MaxValue = 550
                    });

            var numbers = values.Cast<double>();

            var min = numbers.Min();
            var max = numbers.Max();

            Assert.True(min >= 400);
            Assert.True(max <= 550);
        }

        [Fact]
        public void Should_generate_numbers_lower_than_max()
        {
            var values =
                CreateManyScalars(
                    new NumberFieldPropertiesDto
                    {
                        MaxValue = 550
                    });

            var numbers = values.Cast<double>();

            var min = numbers.Min();
            var max = numbers.Max();

            Assert.True(min >= 450);
            Assert.True(max <= 550);
        }

        [Fact]
        public void Should_generate_numbers_greater_than_min()
        {
            var values =
                CreateManyScalars(
                    new NumberFieldPropertiesDto
                    {
                        MinValue = 120
                    });

            var numbers = values.Cast<double>();

            var min = numbers.Min();
            var max = numbers.Max();

            Assert.True(min >= 120);
            Assert.True(max <= 220);
        }

        [Fact]
        public void Should_generate_numbers_with_default_range()
        {
            var values =
                CreateManyScalars(
                    new NumberFieldPropertiesDto());

            var numbers = values.Cast<double>();

            var min = numbers.Min();
            var max = numbers.Max();

            Assert.True(min >= 0);
            Assert.True(max <= 100);
        }

        [Fact]
        public void Should_generate_tags_with_allowed_values()
        {
            var values =
                CreateManyStringTags(
                    new TagsFieldPropertiesDto
                    {
                        AllowedValues = new List<string> { "foo", "bar" }
                    });

            var distinct = values.SelectMany(x => x).Distinct().ToList();

            Assert.Equal(2, distinct.Count);
            Assert.Contains("foo", distinct);
            Assert.Contains("bar", distinct);
        }

        [Fact]
        public void Should_generate_tags_between_min_and_max_items()
        {
            var values =
                CreateManyStringTags(
                    new TagsFieldPropertiesDto
                    {
                        MinItems = 10,
                        MaxItems = 30
                    });

            var min = values.Min(x => x.Count);
            var max = values.Max(x => x.Count);

            Assert.True(min >= 10);
            Assert.True(max <= 30);
        }

        [Fact]
        public void Should_generate_tags_between_with_less_than_max()
        {
            var values =
                CreateManyStringTags(
                    new TagsFieldPropertiesDto
                    {
                        MaxItems = 22
                    });

            var min = values.Min(x => x.Count);
            var max = values.Max(x => x.Count);

            Assert.True(min >= 18);
            Assert.True(max <= 22);
        }

        [Fact]
        public void Should_generate_tags_between_with_more_than_min()
        {
            var values =
                CreateManyStringTags(
                    new TagsFieldPropertiesDto
                    {
                        MinItems = 44
                    });

            var min = values.Min(x => x.Count);
            var max = values.Max(x => x.Count);

            Assert.True(min >= 44);
            Assert.True(max <= 48);
        }

        [Fact]
        public void Should_generate_tags_between_with_default_sizes()
        {
            var values =
                CreateManyStringTags(
                    new TagsFieldPropertiesDto());

            var min = values.Min(x => x.Count);
            var max = values.Max(x => x.Count);

            Assert.True(min >= 1);
            Assert.True(max <= 5);
        }

        [Fact]
        public void Should_generate_string_with_allowed_values()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        AllowedValues = new List<string> { "foo", "bar" }
                    });

            Assert.Equal(2, values.Count);
            Assert.Contains("foo", values);
            Assert.Contains("bar", values);
        }

        [Fact]
        public void Should_generate_string_as_color()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        Editor = StringFieldEditor.Color
                    });

            var strings = values.Cast<string>();

            Assert.True(strings.All(x => x.StartsWith("#", StringComparison.OrdinalIgnoreCase) && x.Length == 7));
        }

        [Fact]
        public void Should_generate_string_equals_to_max_length()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        MaxLength = 20
                    });

            var strings = values.Cast<string>();

            Assert.Equal(new[] { 20 }, strings.Select(x => x.Length).Distinct().ToArray());
        }

        [Fact]
        public void Should_generate_string_equals_to_short_max_length()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        MaxLength = 2
                    });

            var strings = values.Cast<string>();

            Assert.Equal(new[] { 2 }, strings.Select(x => x.Length).Distinct().ToArray());
        }

        [Fact]
        public void Should_generate_string_equals_default_max_length()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        Editor = StringFieldEditor.Input
                    });

            var strings = values.Cast<string>();

            Assert.Equal(new[] { 100 }, strings.Select(x => x.Length).Distinct().ToArray());
        }

        [Fact]
        public void Should_generate_string_equals_default_html_max_length()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        Editor = StringFieldEditor.Markdown
                    });

            var strings = values.Cast<string>();

            Assert.Equal(new[] { 1000 }, strings.Select(x => x.Length).Distinct().ToArray());
        }

        [Fact]
        public void Should_generate_string_equals_default_markdown_max_length()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        Editor = StringFieldEditor.Markdown
                    });

            var strings = values.Cast<string>();

            Assert.Equal(new[] { 1000 }, strings.Select(x => x.Length).Distinct().ToArray());
        }

        [Fact]
        public void Should_generate_slugify_text()
        {
            var values =
                CreateManyScalars(
                    new StringFieldPropertiesDto
                    {
                        MaxLength = 20,
                        MinLength = 20,
                        Editor = StringFieldEditor.Slug
                    });

            var strings = values.Cast<string>().Distinct();

            Assert.Equal(new[] { "lorem-ipsum-dolorxxx" }, strings);
        }

        [Fact]
        public void Should_generate_json()
        {
            var values =
                CreateValue(
                    new JsonFieldPropertiesDto());

            var obj = values as JObject;

            Assert.NotNull(obj);
            Assert.Single(obj);
            Assert.True(obj?.ContainsKey("value"));
        }

        [Fact]
        public void Should_generate_geolocation()
        {
            var values =
                CreateValue(
                    new GeolocationFieldPropertiesDto());

            var obj = values as JObject;

            Assert.NotNull(obj);
            Assert.Equal(2, obj?.Count);
            Assert.True(obj?.ContainsKey("latitude"));
            Assert.True(obj?.ContainsKey("longitude"));
        }

        private static HashSet<List<string>> CreateManyStringTags(FieldPropertiesDto field)
        {
            var values = new HashSet<List<string>>();

            for (var i = 0; i < 1000; i++)
            {
                values.Add(CreateValue(field)!.ToObject<List<string>>()!);
            }

            return values;
        }

        private static HashSet<object?> CreateManyScalars(FieldPropertiesDto field)
        {
            var values = new HashSet<object?>();

            for (var i = 0; i < 1000; i++)
            {
                values.Add(CreateScalar(field));
            }

            return values;
        }

        private static object? CreateScalar(FieldPropertiesDto field)
        {
            var value = CreateValue(field);

            return (value as JValue)?.Value;
        }

        private static JToken? CreateValue(FieldPropertiesDto field)
        {
            var schema = new SchemaDto
            {
                Fields = new List<FieldDto>
                {
                    new FieldDto
                    {
                        Name = "field",
                        Properties = field,
                        Partitioning = "invariant"
                    }
                }
            };

            var sut = new TestDataGenerator(schema, null!);

            var data = sut.GenerateTestData();

            return data["field"]["iv"];
        }
    }
}
