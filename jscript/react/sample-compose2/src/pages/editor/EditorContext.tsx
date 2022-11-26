import React, { createContext } from "react";

export type ContentRenderers = { [id: string]: React.ComponentType<{ content: any }> };

interface ContentCache {
    get(id: string | undefined): Promise<any | null>;

    set(id: string, content: any): void;

    load(ids: string[]): void;
}

export class NoopContentCache implements ContentCache {
    public get() {
        return Deferred.value(null).promise;
    }

    public set() {
        return;
    }

    public load() {
        return;
    }
}

export class DynamicContentCache implements ContentCache {
    private readonly cache: { [id: string]: Deferred<any | null> } = {};

    constructor(
        private readonly apiUrl: string,
        private readonly accessToken: string,
        private readonly appName: string
    ) {
    }

    public get(id: string | undefined) {
        if (!id) {
            return Deferred.value(null).promise;
        }

        if (!this.cache[id]) {
            this.load([id]);
        }

        return this.cache[id].promise;
    }

    public set(id: string, content: any) {
        this.cache[id] = Deferred.value(content);
    }

    public async load(ids: string[]) {
        if (!ids || ids.length === 0) {
            return;
        }

        const unloaded = ids.filter(id => !this.cache[id]);

        if (unloaded.length === 0) {
            return;
        }

        for (const id of ids) {
            this.cache[id] = new Deferred<any>();
        }

        try {
            const contentUrl = `${this.apiUrl}/content/${this.appName}/?ids=${ids.join(', ')}`;

            const response = await fetch(contentUrl, { 
                headers: {
                    'Authorization': `Bearer ${this.accessToken}`,
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

export class Deferred<T> {
    public readonly promise: Promise<T>;
    private resolveFn!: (value: T | PromiseLike<T>) => void;
    private rejectFn!: (reason?: any) => void;

    public constructor() {
        this.promise = new Promise<T>((resolve, reject) => {
            this.resolveFn = resolve
            this.rejectFn = reject
        })
    }

    public reject(reason?: any) {
        this.rejectFn(reason);
        return this;
    }

    public resolve(param: T) {
        this.resolveFn(param);
        return this;
    }

    public static value<T>(value: T) {
        return new Deferred<T>().resolve(value);
    }
}

export function isArray(value: any): value is any[] {
    return Array.isArray(value);
}

export function isString(value: any): value is string {
    return typeof value === 'string' || value instanceof String;
}

export function isObject(value: any): value  is Object {
    return value && typeof value === 'object' && value.constructor === Object;
}

export function resolveIds(value: any) {
    const ids: { [id: string]: boolean } = {};

    const resolve = (value: any) => {
        if (isArray(value)) {
            for (const item of value) {
                resolve(item);
            }
        } else if (isObject(value)) {
            if (value.plugin?.id === 'squidexContent') {
                const data = value.valueI18n;

                if (isObject(data)) {
                    for (const localeData of Object.values(data)) {
                        const id = localeData;

                        if (isString(id)) {
                            ids[id] = true;
                        }
                    }
                }
            } else {
                for (const item of Object.values(value)) {
                    resolve(item);
                }
            }
        }
    };

    resolve(value);

    return Object.keys(ids);
}

export type EditorContextType = {
    field?: SquidexFormField;

    contentCache: ContentCache;
    contentRenderers: ContentRenderers;
}

export const EditorContext = createContext<EditorContextType>({ 
    contentCache: new NoopContentCache(), 
    contentRenderers: {} 
});