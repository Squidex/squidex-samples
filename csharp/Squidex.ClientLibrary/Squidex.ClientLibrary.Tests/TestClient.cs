// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary.Tests
{
    public static class TestClient
    {
        public const string AppName = "integration-tests";
        public const string ClientId = "root";
        public const string ClientSecret = "xeLd6jFxqbXJrfmNLlO2j1apagGGGSyZJhFnIuHp4I0=";
        public const string SchemaName = "numbers";
        public const string ServerUrl = "http://localhost:5000";
        public const string FieldText = "text";
        public const string FieldValue = "value";

        public static readonly SquidexClientManager ClientManager =
            new SquidexClientManager("http://localhost:5000", AppName, ClientId, ClientSecret)
            {
                ReadResponseAsString = true
            };

        public static SquidexClient<TestEntity, TestEntityData> Build()
        {
            return ClientManager.GetClient<TestEntity, TestEntityData>(SchemaName);
        }

        public static async Task SetupAsync()
        {
            var apps = ClientManager.CreateAppsClient();

            try
            {
                await apps.PostAppAsync(new CreateAppDto
                {
                    Name = AppName
                });

                var schemas = ClientManager.CreateSchemasClient();

                await schemas.PostSchemaAsync(AppName, new CreateSchemaDto
                {
                    Name = SchemaName,
                    Fields = new List<UpsertSchemaFieldDto>
                    {
                        new UpsertSchemaFieldDto
                        {
                            Name = FieldValue,
                            Properties = new NumberFieldPropertiesDto()
                        },
                        new UpsertSchemaFieldDto
                        {
                            Name = FieldText,
                            Properties = new StringFieldPropertiesDto()
                        }
                    },
                    IsPublished = true
                });
            }
            catch (SquidexManagementException ex)
            {
                if (ex.StatusCode != 400)
                {
                    throw;
                }
            }
        }
    }
}
