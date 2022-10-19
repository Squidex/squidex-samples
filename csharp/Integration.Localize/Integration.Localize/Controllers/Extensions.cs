// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Integration.Localize.Controllers
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            TSource[]? bucket = null;

            var bucketIndex = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                {
                    bucket = new TSource[size];
                }

                bucket[bucketIndex++] = item;

                if (bucketIndex != size)
                {
                    continue;
                }

                yield return bucket;

                bucket = null;
                bucketIndex = 0;
            }

            if (bucket != null && bucketIndex > 0)
            {
                yield return bucket.Take(bucketIndex);
            }
        }
    }
}
