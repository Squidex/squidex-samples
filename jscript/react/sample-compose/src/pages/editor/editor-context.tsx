import React, { createContext, useContext } from 'react';
import { isArray, isObject, isString, ContentCache } from '../..//shared';

export type ContentType = {
    // The display name.
    name: string;

    // The actual rendererer.
    renderer: React.ComponentType<{ content: any }>;
};

export type ContentTypes = { [id: string]: ContentType };

export function resolveIds(value: any) {
    const idsContents: { [id: string]: boolean } = {};
    const idsAssets: { [id: string]: boolean } = {};

    const addIds = (value: any, ids: { [id: string]: boolean }) => {
        const data = value.dataI18n;

        if (!isObject(data)) {
            return;
        }

        for (const localeData of Object.values(data)) {
            const id = (localeData as any)?.['id'];

            if (isString(id)) {
                ids[id] = true;
            }
        }
    };

    const resolve = (value: any) => {
        if (isArray(value)) {
            for (const item of value) {
                resolve(item);
            }
        } else if (isObject(value)) {
            const id = value.plugin?.id;
             
            if (isString(id) && id.indexOf('squidexContent') === 0) {
                addIds(value, idsContents);
            } else if (isString(id) && id.indexOf('squidexAsset') === 0) {
                addIds(value, idsContents);
            } else {
                for (const item of Object.values(value)) {
                    resolve(item);
                }
            }
        }
    };

    resolve(value);

    return { contentIds: Object.keys(idsContents), assetIds: Object.keys(idsAssets) };
}

export type EditorContextType = {
    field?: SquidexFormField;
    contentCache: ContentCache;
    contentTypes: ContentTypes;
}

export const EditorContext = createContext<EditorContextType>({ 
    contentCache: null!, 
    contentTypes: {} 
});

export function useEditorContext() {
    return useContext(EditorContext);
}