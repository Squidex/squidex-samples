// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.ClientLibrary
{
    public static class SquidexClientExtensions
    {
        public static async Task<SquidexEntities<TEntity, TData>> GetAllAsync<TEntity, TData>(this SquidexClient<TEntity, TData> client, int batchSize = 200)
            where TEntity : SquidexEntityBase<TData>
            where TData : class, new()
        {
            var query = new ContentQuery { Top = batchSize };

            var entities = new SquidexEntities<TEntity, TData>();
            do
            {
                var getResult = await client.GetAsync(query);

                entities.Total = getResult.Total;
                entities.Items.AddRange(getResult.Items);

                query.Skip = entities.Items.Count;
            }
            while (query.Skip < entities.Total);

            return entities;
        }

        [Obsolete]
        public static async Task<AssetsDto> GetAllAssetsAsync(this IAssetsClient assetClient, string app, int batchSize = 200)
        {
            var query = new AssetQuery { Top = batchSize, Skip = 0 };

            var assetItems = new List<AssetDto>();

            long total;
            do
            {
                var getResult = await assetClient.GetAssetsAsync(app, query);

                total = getResult.Total;
                assetItems.AddRange(getResult.Items);

                query.Skip = assetItems.Count;
            }
            while (query.Skip < assetItems.Count);

            return new AssetsDto { Total = total, Items = assetItems };
        }

        [Obsolete]
        public static async Task<AssetEntities> GetAllAssetsAsync(this SquidexAssetClient assetClient, int batchSize = 200)
        {
            var query = new ContentQuery { Top = batchSize, Skip = 0 };

            var entities = new AssetEntities();
            do
            {
                var getResult = await assetClient.GetAssetsAsync(query);

                entities.Total = getResult.Total;
                entities.Items.AddRange(getResult.Items);

                query.Skip = entities.Items.Count;
            }
            while (query.Skip < entities.Total);

            return entities;
        }
    }
}