import divider from '@react-page/plugins-divider';
import image from '@react-page/plugins-image';
import slate from '@react-page/plugins-slate';
import spacer from '@react-page/plugins-spacer';
import Editor from '@react-page/editor';
import { Hotel, Destination, useApiContext, ContentCache } from './../../shared';
import { ContentTypes, EditorContext } from './editor-context';
import { squidexImage } from './SquidexImage';
import { squidexContent } from './SquidexContent';
import '@react-page/editor/lib/index.css';
import './EditorPage.css';
import { useEffect, useMemo } from 'react';
import { useState } from 'react';

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