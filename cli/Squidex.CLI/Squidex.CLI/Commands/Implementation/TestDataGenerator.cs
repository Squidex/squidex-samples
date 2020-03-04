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
using Slugify;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation
{
    public sealed class TestDataGenerator
    {
        private readonly SlugHelper slugify = new SlugHelper();
        private readonly SchemaDetailsDto schema;
        private readonly AppLanguagesDto languages;
        private readonly Random random = new Random();

        public TestDataGenerator(SchemaDetailsDto schema, AppLanguagesDto languages)
        {
            this.schema = schema;
            this.languages = languages;
        }

        public DummyData GenerateTestData()
        {
            var data = new DummyData();

            foreach (var field in schema.Fields)
            {
                var fieldData = new Dictionary<string, JToken>();

                if (field.Partitioning == "invariant")
                {
                    var value = GenerateValue(field);

                    fieldData["iv"] = value;
                }
                else
                {
                    foreach (var language in languages.Items)
                    {
                        var value = GenerateValue(field);

                        fieldData[language.Iso2Code] = value;
                    }
                }

                data.Add(field.Name, fieldData);
            }

            return data;
        }

        private JToken GenerateValue(FieldDto field)
        {
            switch (field.Properties)
            {
                case BooleanFieldPropertiesDto booleanField:
                    {
                        if (booleanField.IsRequired)
                        {
                            var value = random.Next(2);

                            return value == 1;
                        }
                        else
                        {
                            var value = random.Next(3);

                            return value switch
                            {
                                1 => true,
                                2 => false,
                                _ => null,
                            };
                        }
                    }

                case DateTimeFieldPropertiesDto dateTimeField:
                    {
                        var min = DateTimeOffset.UtcNow;
                        var max = DateTimeOffset.UtcNow.AddDays(30);

                        if (dateTimeField.MinValue.HasValue && dateTimeField.MaxValue.HasValue)
                        {
                            min = dateTimeField.MinValue.Value;
                            max = dateTimeField.MaxValue.Value;
                        }
                        else if (dateTimeField.MinValue.HasValue)
                        {
                            min = dateTimeField.MinValue.Value;
                            max = min.AddDays(30);
                        }
                        else if (dateTimeField.MaxValue.HasValue)
                        {
                            max = dateTimeField.MaxValue.Value;
                            min = max.AddDays(-30);
                        }

                        var range = max - min;

                        var result = min.AddMinutes(random.Next(0, (int)range.TotalMinutes)).UtcDateTime;

                        if (dateTimeField.Editor == DateTimeFieldEditor.Date)
                        {
                            result = result.Date;
                        }

                        return result;
                    }

                case GeolocationFieldPropertiesDto _:
                    {
                        var lat = random.Next(-90, 90);
                        var lon = random.Next(-180, 180);

                        return new JObject(
                            new JProperty("latitude",
                                lat),
                            new JProperty("longitude",
                                lon));
                    }

                case JsonFieldPropertiesDto _:
                    {
                        return new JObject(
                            new JProperty("value",
                                LoremIpsum.GetWord(random)));
                    }

                case NumberFieldPropertiesDto numberField:
                    {
                        if (numberField.AllowedValues?.Count > 0)
                        {
                            return GetRandomValue(numberField.AllowedValues);
                        }

                        var value = GetRandom(numberField.MinValue, numberField.MaxValue, 0, 100);

                        return Math.Round(value, 2);
                    }

                case StringFieldPropertiesDto stringField:
                    {
                        if (!string.IsNullOrWhiteSpace(stringField.Pattern))
                        {
                            throw new NotSupportedException("Patterns validation for string fields are not supported.");
                        }

                        if (stringField.AllowedValues?.Count > 0)
                        {
                            return GetRandomValue(stringField.AllowedValues);
                        }

                        if (stringField.Editor == StringFieldEditor.Color)
                        {
                            var r = random.Next(0, 0xFF);
                            var g = random.Next(0, 0xFF);
                            var b = random.Next(0, 0xFF);

                            return $"#{r:x2}{g:x2}{b:x2}";
                        }

                        var max = 100;

                        if (stringField.MaxLength.HasValue)
                        {
                            max = stringField.MaxLength.Value;
                        }
                        else if (stringField.Editor == StringFieldEditor.RichText || stringField.Editor == StringFieldEditor.Markdown)
                        {
                            max = 1000;
                        }

                        var result = LoremIpsum.Text(max, stringField.Editor == StringFieldEditor.RichText);

                        if (stringField.Editor == StringFieldEditor.Slug)
                        {
                            result = slugify.GenerateSlug(result).Replace(".", "x");
                        }

                        return result;
                    }

                case TagsFieldPropertiesDto tagsField:
                    {
                        var items = (int)GetRandom(tagsField.MinItems, tagsField.MaxItems, 1, 5);

                        var result = new JArray();

                        for (var i = 0; i < items; i++)
                        {
                            string value;

                            if (tagsField.AllowedValues?.Count > 0)
                            {
                                value = GetRandomValue(tagsField.AllowedValues);
                            }
                            else
                            {
                                value = LoremIpsum.GetWord(random);
                            }

                            result.Add(value);
                        }

                        return result;
                    }
            }

            throw new NotSupportedException($"Field type {field.Properties.GetType().Name} for field '{field.Name}' is not supported.");
        }

        private T GetRandomValue<T>(ICollection<T> source)
        {
            return source.ElementAt(random.Next(0, source.Count));
        }

        private double GetRandom(double? minValue, double? maxValue, double defaultMin, double defaultMax)
        {
            var min = defaultMin;
            var max = defaultMax;

            var defaultRange = (defaultMax - defaultMin);

            if (minValue.HasValue && maxValue.HasValue)
            {
                min = minValue.Value;
                max = maxValue.Value;
            }
            else if (minValue.HasValue)
            {
                min = minValue.Value;
                max = min + defaultRange;
            }
            else if (maxValue.HasValue)
            {
                max = maxValue.Value;
                min = max - defaultRange;
            }

            var value = random.NextDouble();

            return min + (value * (max - min));
        }
    }
}
