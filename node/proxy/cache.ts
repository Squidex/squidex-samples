import redis from 'redis';
import { promisify } from 'util';
import config from './config';

const redisClient = redis.createClient({
    url: config.redis.url
});

const redisGet =
    promisify(redisClient.get)
        .bind(redisClient);

const redisGetRange =
    promisify(redisClient.lrange)
        .bind(redisClient);

export function computeCacheKey(url: string, headers: { [key: string]: string | string[] | undefined }) {
    let cacheKey = url;

    for (const key in headers) {
        if (key.startsWith('X-')) {
            cacheKey += '&';
            cacheKey += key;
            cacheKey += '=';

            const value = headers[key];

            if (typeof value === 'string') {
                cacheKey += value;
            } else if (value) {
                cacheKey += value.join(';');
            }
        }
    }

    return cacheKey.replace(/:/g, '_')
}

export async function getFromCache<T>(cacheKey: string): Promise<T | null> {
    let cached = await redisGet(cacheKey);

    if (cached) {
        try {
            return JSON.parse(cached);
        } catch(ex) {
            return null;
        }
    }

    return null;
}

export async function addToCache(cacheKey: string, value: any, surrogateKeys: string[], duration: number) {
    redisClient.set(cacheKey, JSON.stringify(value), 'EX', duration);

    for (const surrogateKey of surrogateKeys) {
        redisClient.lpush(surrogateKey, cacheKey);
    }
}

export function purgeByKey(cacheKey: string) {
    redisClient.del(cacheKey);
}

export async function purgeBySurrogateKey(surrogateKey: string) {
    try {
        const batchSize = 1000;

        let offset = 0;

        while (true) {
            const urls = await redisGetRange(surrogateKey, offset, batchSize);

            for (const url of urls) {
                redisClient.del(url);
            }

            if (urls.length < batchSize) {
                break;
            } else {
                offset += batchSize;
            }
        }
    } finally {
        redisClient.del(surrogateKey);
    }
}