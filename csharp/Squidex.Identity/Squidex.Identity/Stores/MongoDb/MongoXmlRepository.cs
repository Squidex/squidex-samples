// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squidex.Identity.Stores.MongoDb
{
    public sealed class MongoXmlRepository : IXmlRepository
    {
        private static readonly UpdateOptions Upsert = new UpdateOptions { IsUpsert = true };
        private readonly IMongoCollection<MongoXmlDocument> collection;

        public MongoXmlRepository(IMongoDatabase database)
        {
            collection = database.GetCollection<MongoXmlDocument>("Identity_XmlRepository");
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return collection.Find(new BsonDocument()).ToList().Select(ConvertBack).ToList();
        }

        private static XElement ConvertBack(MongoXmlDocument x)
        {
            return XElement.Parse(x.Xml);
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            collection.UpdateOne(x => x.Id == friendlyName,
                Builders<MongoXmlDocument>.Update.Set(x => x.Xml, element.ToString()),
                Upsert);
        }
    }
}
