// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Xunit;

namespace Squidex.ClientLibrary.Tests
{
    public class DynamicContentTests
    {
        [Fact]
        public void Should_serialize_flat_data()
        {
            var source = JsonConvert.SerializeObject(new
            {
                data = new
                {
                    myField = 42
                }
            });

            var result = JsonConvert.DeserializeObject<DynamicContent>(source)!;

            Assert.Equal(42, (int)result.Data["myField"]);
        }

        [Fact]
        public void Should_serialize_normal_data()
        {
            var source = JsonConvert.SerializeObject(new
            {
                data = new
                {
                    myField = new
                    {
                        iv = 42
                    }
                }
            });

            var result = JsonConvert.DeserializeObject<DynamicContent>(source)!;

            Assert.Equal(42, (int)result.Data["myField"]!["iv"]!);
        }
    }
}
