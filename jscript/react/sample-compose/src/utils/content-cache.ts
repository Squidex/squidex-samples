import { ApiContextType } from './api-context';
import { Deferred } from './deferred';
import { isArray, isStringArray } from './types';

export class ContentCache {
    private readonly cache: { [id: string]: Deferred<any | null> } = {};

    constructor(
        private readonly api: ApiContextType,
    ) {
    }

    public get(id: string | undefined, needsReload = false) {
        if (!id) {
            return Deferred.value(null).promise;
        }

        if (!this.cache[id] || needsReload) {
            this.load([id], needsReload);
        }

        return this.cache[id].promise;
    }

    public set(id: string, content: any) {
        this.cache[id] = Deferred.value(content);
    }

    public setMany(items: any[]) {
        if (!isArray(items)) {
            return;
        }

        for (const item of items) {
            this.set(item.id, item);
        }
    }

    public async load(ids: string[] | undefined, needsReload = false) {
        if (!isStringArray(ids) || ids.length === 0) {
            return;
        }

        const unloaded = ids.filter(id => needsReload || !this.cache[id]);

        if (unloaded.length === 0) {
            return;
        }

        for (const id of ids) {
            this.cache[id] = new Deferred<any>();
        }

        try {
            const accessToken = await this.api.accessToken(false);
            
            if (!accessToken) {
                return;
            }

            const contentUrl = `${this.api.apiUrl}/api/content/${this.api.appName}/?ids=${ids.join(', ')}`;

            const response = await fetch(contentUrl, { 
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                }
            });

            if (!response.ok) {
                for (const id of ids) {
                    this.cache[id].resolve(null);
                }
                return;
            }

            const contents = await response.json() as { items: any[] };

            for (const id of ids) {
                this.cache[id].resolve(contents.items.find(x => x.id === id));
            }
            return 
        } catch {
            for (const id of ids) {
                this.cache[id].resolve(null);
            }
        }
    }
}