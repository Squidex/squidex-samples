// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Squidex.Identity.Stores.MongoDb
{
    public static class MongoExtensions
    {
        public static void AddMongoDB(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoConfiguration = configuration.GetValue<string>("store:mongoDb:configuration");
            var mongoDatabaseName = configuration.GetValue<string>("store:mongoDb:database");

            if (string.IsNullOrWhiteSpace(mongoConfiguration))
            {
                throw new ApplicationException("You have to define the MongoDB collection with 'store:mongoDb:configuration'");
            }

            if (string.IsNullOrWhiteSpace(mongoDatabaseName))
            {
                throw new ApplicationException("You have to define the MongoDB database with 'store:mongoDb:database'");
            }

            var mongoClient = (IMongoClient)new MongoClient(mongoConfiguration);
            var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

            services.AddSingleton(mongoClient);
            services.AddSingleton(mongoDatabase);

            services.AddSingleton<IXmlRepository,
                MongoXmlRepository>();

            services.AddSingleton<IPersistedGrantStore,
                MongoPersistedGrantStore>();
        }
    }
}
