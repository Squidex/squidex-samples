// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Squidex.Identity.Stores.MongoDb
{
    public sealed class MongoPersistedGrantStore : IPersistedGrantStore
    {
        private static readonly UpdateOptions Upsert = new UpdateOptions { IsUpsert = true };
        private readonly IMongoCollection<PersistedGrant> collection;

        static MongoPersistedGrantStore()
        {
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Key));
            });
        }

        public MongoPersistedGrantStore(IMongoDatabase database)
        {
            collection = database.GetCollection<PersistedGrant>("Identity_PersistedGrants");

            collection.Indexes.CreateOne(
                Builders<PersistedGrant>.IndexKeys
                    .Ascending(x => x.SubjectId)
                    .Ascending(x => x.ClientId)
                    .Ascending(x => x.Type));
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = await collection.Find(x => x.SubjectId == subjectId).ToListAsync();

            return result;
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            return collection.Find(x => x.Key == key).FirstOrDefaultAsync();
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            return collection.DeleteManyAsync(x => x.SubjectId == subjectId && x.ClientId == clientId);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            return collection.DeleteManyAsync(x => x.SubjectId == subjectId && x.ClientId == clientId && x.Type == type);
        }

        public Task RemoveAsync(string key)
        {
            return collection.DeleteManyAsync(x => x.Key == key);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            return collection.ReplaceOneAsync(x => x.Key == grant.Key, grant, Upsert);
        }
    }
}
