import { useCallback, useEffect, useMemo, useState } from 'react'
import Editor, { Value } from '@react-page/editor';
import { RefreshContext, Deferred, PageLayout, ContentCache, ApiContext, RefreshListener } from './../../shared';
import { EditorContext, resolveIds } from './editor-context';
import { cellPlugins, contentTypes } from './config';
import '@react-page/editor/lib/index.css';
import './EditorPage.css';

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

    const api = useMemo(() => {
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

    const refreshListener = useMemo(() => {
        if (!api) {
            return null;
        } 
        
        return new RefreshListener(api);
    }, [api]);

    const contentCache = useMemo(() => {
        if (!api) {
            return null;
        } 
        
        return new ContentCache(api);
    }, [api]);

    useEffect(() => {
        if (!contentCache) {
            return;
        }

        contentCache.load(fieldInput?.contentIds);
    }, [contentCache, fieldInput]);

    if (!contentCache || !api) {
        return null;
    }

    return (
        <ApiContext.Provider value={api}>
            <RefreshContext.Provider value={{ listener: refreshListener }}>
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
            </RefreshContext.Provider>
        </ApiContext.Provider>
    );
};