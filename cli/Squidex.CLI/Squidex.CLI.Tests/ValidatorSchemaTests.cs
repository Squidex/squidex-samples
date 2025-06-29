using Squidex.CLI.Commands.Implementation.AI;
using Xunit;

namespace Squidex.CLI;

public class ValidatorSchemaTests
{
    [Fact]
    public void Should_return_error_when_fields_is_empty()
    {
        var schema = new SimplifiedSchema
        {
            Name = "my-schema",
            Fields = [],
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["fields: No field defined"], errors);
    }

    [Fact]
    public void Should_return_error_when_fields_is_null()
    {
        var schema = new SimplifiedSchema
        {
            Name = "my-schema",
            Fields = null!,
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["fields: No field defined"], errors);
    }

    [Fact]
    public void Should_return_error_when_schema_name_not_a_slug()
    {
        var schema = new SimplifiedSchema
        {
            Name = "my schema",
            Fields =
            [
                new SimplifiedField
                {
                    Name = "myField",
                    Type = SimplifiedFieldType.Text,
                },
            ],
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["name: Not a valid slug"], errors);
    }

    [Fact]
    public void Should_return_error_when_schema_name_not_defined()
    {
        var schema = new SimplifiedSchema
        {
            Name = string.Empty,
            Fields =
            [
                new SimplifiedField
                {
                    Name = "myField",
                    Type = SimplifiedFieldType.Text,
                },
            ],
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["name: Value is required"], errors);
    }

    [Fact]
    public void Should_return_error_when_field_name_is_not_valid_property_name()
    {
        var schema = new SimplifiedSchema
        {
            Name = "my-schema",
            Fields =
            [
                new SimplifiedField
                {
                    Name = "my field",
                    Type = SimplifiedFieldType.Text,
                },
            ],
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["fields[0].name: Not a property name"], errors);
    }

    [Fact]
    public void Should_return_error_when_field_name_not_defined()
    {
        var schema = new SimplifiedSchema
        {
            Name = "my-schema",
            Fields =
            [
                new SimplifiedField
                {
                    Name = string.Empty,
                    Type = SimplifiedFieldType.Text,
                },
            ],
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["fields[0].name: Value is required"], errors);
    }

    [Fact]
    public void Should_return_error_when_min_value_is_greater_than_max_value()
    {
        var schema = new SimplifiedSchema
        {
            Name = "my-schema",
            Fields =
            [
                new SimplifiedField
                {
                    Name = "myField",
                    MinValue = 42,
                    MaxValue = 13,
                    Type = SimplifiedFieldType.Text,
                },
            ],
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["fields[0].minValue: Must be smaller than max value"], errors);
    }

    [Fact]
    public void Should_return_error_when_min_length_is_greater_than_max_length()
    {
        var schema = new SimplifiedSchema
        {
            Name = "my-schema",
            Fields =
            [
                new SimplifiedField
                {
                    Name = "myField",
                    MinLength = 42,
                    MaxLength = 13,
                    Type = SimplifiedFieldType.Text,
                },
            ],
        };

        var errors = ValidateSchema(schema);

        Assert.Equal(["fields[0].minLength: Must be smaller than max length"], errors);
    }

    private static IReadOnlyList<string> ValidateSchema(SimplifiedSchema schema)
    {
        var validator = new Validator(schema, ["en", "de"]);

        return validator.ValidateSchema();
    }
}
