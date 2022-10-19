// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using System.Text.Json;
using Squidex.ClientLibrary;

namespace Integration.Localize.Controllers
{
    [ErrorActionFilter]
    public partial class DefaultController : ControllerBase
    {
        private static class MetaFields
        {
            public const string ContentId = nameof(ContentId);
            public const string ContentField = nameof(ContentField);
            public const string SchemaId = nameof(SchemaId);
            public const string SchemaName = nameof(SchemaName);
            public const string DateCreated = nameof(DateCreated);
            public const string DateUpdated = nameof(DateUpdated);
        }

        private ISquidexClientManager BuildClientManager()
        {
            if (!Request.Headers.TryGetValue("GE-Config", out var config))
            {
                throw new InvalidOperationException("GE-Config header is not found.");
            }

            var decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(config));
            var decodedObject = JsonSerializer.Deserialize<Dictionary<string, string>>(decodedString);

            return BuildClientManager(decodedObject);
        }

        private static ISquidexClientManager BuildClientManager(IDictionary<string, string>? properties)
        {
            if (properties == null)
            {
                throw new InvalidOperationException("Properties is null.");
            }

            var options = new SquidexOptions();

            if (!properties.TryGetValue("ClientId", out var clientId))
            {
                throw new InvalidOperationException("ClientId is required.");
            }
            else
            {
                options.ClientId = clientId;
            }

            if (!properties.TryGetValue("ClientSecret", out var clientSecret))
            {
                throw new InvalidOperationException("ClientSecret is required.");
            }
            else
            {
                options.ClientSecret = clientSecret;
            }

            if (!properties.TryGetValue("AppName", out var appName))
            {
                throw new InvalidOperationException("AppName is required.");
            }
            else
            {
                options.AppName = appName;
            }

            if (properties.TryGetValue("Url", out var url))
            {
                options.Url = url;
            }

            return new SquidexClientManager(options);
        }
    }
}
