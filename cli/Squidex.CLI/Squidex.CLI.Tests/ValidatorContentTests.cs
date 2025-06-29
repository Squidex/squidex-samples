// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;
using Squidex.CLI.Commands.Implementation.AI;
using Xunit;

namespace Squidex.CLI;

public class ValidatorContentTests
{
    private readonly SimplifiedSchema schema = new SimplifiedSchema
    {
        Name = "my-schema",
        Fields =
        [
            new SimplifiedField
            {
                Name = "my-string",
                Type = SimplifiedFieldType.Text,
                MinLength = 10,
                MaxLength = 30,
            },
            new SimplifiedField
            {
                Name = "my-number",
                Type = SimplifiedFieldType.Number,
                MinValue = 10,
                MaxValue = 30,
            },
            new SimplifiedField
            {
                Name = "my-color",
                Type = SimplifiedFieldType.Color,
            },
            new SimplifiedField
            {
                Name = "my-boolean",
                Type = SimplifiedFieldType.Boolean,
            },
            new SimplifiedField
            {
                Name = "my-image",
                Type = SimplifiedFieldType.Image,
            },
            new SimplifiedField
            {
                Name = "my-required",
                IsRequired = true,
            },
            new SimplifiedField
            {
                Name = "my-localized",
                IsLocalized = true,
            },
            new SimplifiedField
            {
                Name = "my-localized-required",
                IsLocalized = true,
                IsRequired = true,
            },
        ],
    };

    [Fact]
    public void Common_should_return_error_if_required_value_is_null()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-required"] = JValue.CreateNull(),
        };

        var errors = ValidateContent(content, false);

        Assert.Equal(["[0].my-required: Value is required"], errors);
    }

    [Fact]
    public void Common_should_return_error_if_field_is_unknown()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-unknown"] = "unknown",
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-unknown: Unknown field"], errors);
    }

    [Fact]
    public void Common_should_return_error_if_required_value_is_not_defined()
    {
        var content = new Dictionary<string, JToken>
        {
        };

        var errors = ValidateContent(content, false);

        Assert.Equal(["[0].my-required: Value is required"], errors);
    }

    [Fact]
    public void Localized_should_return_error_if_field_is_not_an_object()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-localized"] = true,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-localized: Expected object, got boolean"], errors);
    }

    [Fact]
    public void Localized_should_return_error_if_language_is_unknown()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-localized"] = JObject.FromObject(new { es = true }),
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-localized.es: Unknown language"], errors);
    }

    [Fact]
    public void Localized_should_return_error_if_required_and_value_not_defined()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-localized-required"] = JObject.FromObject(new { de = true }),
        };

        var errors = ValidateContent(content, addLocalized: false);

        Assert.Equal(["[0].my-localized-required.en: Value is required"], errors);
    }

    [Fact]
    public void Number_should_return_error_if_value_has_other_type()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-number"] = true,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-number: Expected number, got boolean"], errors);
    }

    [Fact]
    public void Number_should_return_error_if_value_is_smaller_than_min_value()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-number"] = 5,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-number: Expected min value of 10, got 5"], errors);
    }

    [Fact]
    public void Number_should_return_error_if_value_is_larger_than_max_value()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-number"] = 40,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-number: Expected max value of 30, got 40"], errors);
    }

    [Fact]
    public void Numbershould_not_return_error_if_valid()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-number"] = 25,
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Number_should_not_return_error_if_null_value()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-number"] = JValue.CreateNull(),
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Text_should_return_error_if_value_has_other_type()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-string"] = 42,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-string: Expected string, got integer"], errors);
    }

    [Fact]
    public void Text_should_return_error_if_value_is_shorter_than_min_length()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-string"] = new string('x', 5),
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-string: Expected min length of 10, got 5"], errors);
    }

    [Fact]
    public void Text_should_return_error_if_value_is_longer_than_max_length()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-string"] = new string('x', 40),
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-string: Expected max length of 30, got 40"], errors);
    }

    [Fact]
    public void Text_should_not_return_error_if_valid()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-string"] = new string('x', 25),
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Text_should_not_return_error_if_null_value()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-string"] = JValue.CreateNull(),
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Boolean_should_return_error_if_value_has_other_type()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-boolean"] = 42,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-boolean: Expected boolean, got integer"], errors);
    }

    [Fact]
    public void Boolean_should_not_return_error_if_valid()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-boolean"] = true,
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Boolean_should_not_return_error_if_null_value()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-boolean"] = JValue.CreateNull(),
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Color_should_return_error_if_value_has_other_type()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-color"] = 42,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-color: Expected string, got integer"], errors);
    }

    [Fact]
    public void Color_should_return_error_if_value_not_a_color_string()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-color"] = "red",
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-color: Not a valid color"], errors);
    }

    [Fact]
    public void Color_should_not_return_error_if_valid()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-color"] = "#ff0000",
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Color_should_not_return_error_if_null_value()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-color"] = JValue.CreateNull(),
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Image_should_return_error_if_value_has_other_type()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-image"] = 42,
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-image: Expected object with description and file name, got invalid value"], errors);
    }

    [Fact]
    public void Image_should_return_error_if_image_has_no_description()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-image"] = JObject.FromObject(new { fileName = "file.png" }),
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-image: Image has no description"], errors);
    }

    [Fact]
    public void Image_should_return_error_if_image_has_no_file_name()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-image"] = JObject.FromObject(new { description = "Description" }),
        };

        var errors = ValidateContent(content);

        Assert.Equal(["[0].my-image: Image has no file name"], errors);
    }

    [Fact]
    public void Image_should_not_return_error_if_valid()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-image"] = JObject.FromObject(new { description = "Description", fileName = "file.png" }),
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    [Fact]
    public void Image_should_not_return_error_if_null_value()
    {
        var content = new Dictionary<string, JToken>
        {
            ["my-image"] = JValue.CreateNull(),
        };

        var errors = ValidateContent(content);

        Assert.Empty(errors);
    }

    private List<string> ValidateContent(Dictionary<string, JToken> content, bool addRequired = true, bool addLocalized = true)
    {
        if (addRequired)
        {
            content["my-required"] = true;
        }

        if (addLocalized)
        {
            content["my-localized-required"] = JObject.FromObject(new { en = true, de = true });
        }

        var validator = new Validator(schema, ["en", "de"]);

        return [.. validator.ValidateContents([content])];
    }
}
