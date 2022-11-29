import { useEffect, useMemo, useState } from 'react';
import Editor from '@react-page/editor';
import { useApiContext, ContentCache } from './../../shared';
import { EditorContext } from './editor-context';
import { cellPlugins, contentTypes } from './config';
import '@react-page/editor/lib/index.css';
import './EditorPage.css';

export const Viewer = ({ value, preloaded }: { value: any, preloaded: string[] }) => {
    const [loaded, setLoaded] = useState(false);
    const api = useApiContext();

    const contentCache = useMemo(() => {        
        return new ContentCache(api);
    }, [api]);

    useEffect(() => {
        contentCache.setMany(preloaded);
        setLoaded(true);
    }, [contentCache, preloaded]);
    
    if (!loaded) {
        return null;
    }

    return (
        <EditorContext.Provider value={{ 
            contentCache,
            contentTypes
        }}>
            <Editor cellPlugins={cellPlugins} value={value} readOnly />
        </EditorContext.Provider>
    );
};