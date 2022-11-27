import { useCallback, useEffect, useMemo, useState } from 'react'
import divider from '@react-page/plugins-divider';
import image from '@react-page/plugins-image';
import slate from '@react-page/plugins-slate';
import spacer from '@react-page/plugins-spacer';
import Editor, { Value } from '@react-page/editor';
import { ApiContext, Deferred, PageLayout, Hotel, Destination, ContentCache } from './../../shared';
import { ContentTypes, EditorContext, resolveIds } from './editor-context';
import { squidexImage } from './SquidexImage';
import { squidexContent } from './SquidexContent';
import '@react-page/editor/lib/index.css';
import './EditorPage.css';

const contentTypes: ContentTypes = {
    'hotels': {
        renderer: ({ content }) => {
            return (
                <Hotel hotel={content} />
            )
        },
        name: 'Hotel'
    },
    'destination': {
        renderer: ({ content }) => {
            return (
                <Destination destination={content} />
            )
        },
        name: 'Destination'
    },
}

const cellPlugins = [
    divider,
    image,
    slate(),
    spacer,
    squidexImage, 
    ...Object.entries(contentTypes).map(([schema, contentType]) => squidexContent(schema, contentType))
];

type EditorValue = { page: Value, contentIds: string[], assetIds: string[] } | null | undefined;

export const EditorPage = () => {
    const [value, setValue] = useState<EditorValue>(null);
    const [fieldEditor, setFieldEditor] = useState<SquidexFormField | undefined>(undefined);
    const [fieldInput, setFieldInput] = useState<EditorValue>(null);
    const [disabled, setDisabled] = useState<boolean>(false);

    useEffect(() => {
        try {
            const field = new SquidexFormField();

            field.onValueChanged((value) => {
                setValue(value);
                setFieldInput(value);
            });

            field.onDisabled((isDisabled) => {
                setDisabled(isDisabled);
            });

            field.onInit(() => {
                setFieldEditor(field);
            });
        } catch (ex) {
            console.log(`Not embedded in Squidex: ${ex}`);
        }
    }, []);

    const setActualValue = useCallback((page: any) => {
        if (!fieldEditor) {
            return;
        }

        const { contentIds, assetIds } = resolveIds(page);

        fieldEditor.valueChanged({ page, contentIds, assetIds });
        fieldEditor.touched();
        setValue({ page, contentIds, assetIds });
    }, [fieldEditor]);

    const apiContext = useMemo(() => {
        const context = fieldEditor?.getContext();

        if (!context) {
            return null;
        }
        const { appName, user } = context;

        let apiUrl: string = context.apiUrl;

        if (apiUrl.endsWith('/')) {
            apiUrl = apiUrl.substring(0, apiUrl.length - 1);
        }

        if (apiUrl.endsWith('/api')) {
            apiUrl = apiUrl.substring(0, apiUrl.length - 4);
        }

        return { apiUrl, appName, accessToken: () => Deferred.value(user.accessToken).promise };
    }, [fieldEditor]);

    const contentCache = useMemo(() => {
        if (!apiContext) {
            return null;
        } 
        
        return new ContentCache(apiContext);
    }, [apiContext]);

    useEffect(() => {
        if (!contentCache) {
            return;
        }

        contentCache.load(fieldInput?.contentIds);
    }, [contentCache, fieldInput]);

    if (!contentCache) {
        return null;
    }

    return (
        <ApiContext.Provider value={apiContext as any}>
            <EditorContext.Provider value={{ 
                contentCache,
                contentTypes,
                field: fieldEditor 
            }}>
                <PageLayout>
                    <div className='editor-container'>
                        <div className={disabled ? 'disabled' : 'none'}>
                            <Editor cellPlugins={cellPlugins} value={value?.page} onChange={setActualValue} />
                        </div>
                    </div>
                </PageLayout>
            </EditorContext.Provider>
        </ApiContext.Provider>
    );
};