using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public static class SquidexClientExtensions
    {
        public static async Task<SquidexEntities<TEntity, TData>> GetAllAsync<TEntity, TData>(
            this SquidexClient<TEntity, TData> client,
            int batchSize = 200)
            where TEntity : SquidexEntityBase<TData>
            where TData : class, new()
        {
            Guard.NotNull(client, nameof(client));

            int skip = 0;
            var entities = new SquidexEntities<TEntity, TData>();
            do
            {
                var getResult = await client.GetAsync(skip: skip, top: batchSize);

                entities.Total = getResult.Total;
                entities.Items.AddRange(getResult.Items);

                skip += entities.Items.Count;
            }
            while (skip < entities.Total);

            return entities;
        }

        public static async Task<AssetEntities> GetAllAssetsAsync(this SquidexAssetClient assetClient, int batchSize = 200)
        {
            Guard.NotNull(assetClient, nameof(assetClient));

            int skip = 0;
            var entities = new AssetEntities();
            do
            {
                var getResult = await assetClient.GetAssetsAsync(skip: skip, top: batchSize);

                entities.Total = getResult.Total;
                entities.Items.AddRange(getResult.Items);

                skip += entities.Items.Count;
            }
            while (skip < entities.Total);

            return entities;
        }
    }
}