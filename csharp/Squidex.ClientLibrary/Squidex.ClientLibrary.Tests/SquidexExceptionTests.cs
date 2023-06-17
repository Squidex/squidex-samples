// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace Squidex.ClientLibrary.Tests;

public class SquidexExceptionTests
{
    [Fact]
    public void Should_serialize_simple_exception()
    {
        var source = new SquidexException("Message");

        var serialized = SerializeAndDeserialize(source);

        Assert.Equal("Message", serialized.Message);
    }

    [Fact]
    public void Should_serialize_with_status_code()
    {
        var source = new SquidexException("Message", 404);

        var serialized = SerializeAndDeserialize(source);

        Assert.Equal(404, serialized.StatusCode);
    }

    [Fact]
    public void Should_serialize_with_details()
    {
        var details = new ErrorDto
        {
            Message = "My Error"
        };

        var source = new SquidexException<ErrorDto>("Message", 0, details);

        var serialized = SerializeAndDeserialize(source);

        Assert.Equal(details.Message, serialized.Result?.Message);
    }

    private static T SerializeAndDeserialize<T>(T source) where T : notnull
    {
        var formatter = new BinaryFormatter();

        using (var stream = new MemoryStream())
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            formatter.Serialize(stream, source);

            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }
    }
}
