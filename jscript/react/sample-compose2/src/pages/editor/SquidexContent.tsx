import { Button } from '@mui/material';
import ClearIcon from '@mui/icons-material/Clear';
import FilterIcon from '@mui/icons-material/Filter';
import { CellPlugin, CellPluginComponentProps, lazyLoad } from '@react-page/editor';
import { connectField } from 'uniforms';
import { EditorContext, EditorContextType } from './EditorContext';
import { useEffect, useMemo, useRef, useState } from 'react';

type Data = {
    // The content ID.
    id?: string;
}

const ContentIcon = lazyLoad(() => import('@mui/icons-material/ContentCopy'));

export const iconStyle: React.CSSProperties = {
    width: '100%',
    height: 'auto',
    padding: '0',
    color: '#aaa',
    textAlign: 'center',
    minWidth: 64,
    minHeight: 32,
    maxHeight: 64,
};

const ContentHtmLRendererInner = ({ id, state }: { id?: string, state: EditorContextType }) => {
    const [contentItem, setContentItem] = useState<any>(undefined);
    const loaderPromise = useRef<any>();

    const ContentRenderer = useMemo(() => {
        if (!contentItem) {
            return null;
        } else {
            return state.contentRenderers[contentItem.schemaName];
        }
    }, [contentItem, state.contentRenderers]);

    useEffect(() => {
        const loader = loaderPromise.current = state.contentCache.get(id);

        loader.then(content => {
            if (loaderPromise.current === loader) {
                setContentItem(content);
            }
        });
    }, [id, state]);

    if (ContentRenderer && contentItem) {
        return (
            <ContentRenderer content={contentItem} />
        )
    } else {
        return (
            <div>
                <div className="react-page-plugins-content-image-placeholder">
                    <ContentIcon style={iconStyle} />
                </div>
            </div>
        )
    }
};

const ContentHtmlRenderer = (props: CellPluginComponentProps<Data>) => {
    const { id } = props.data;

    return (
        <div>
            <EditorContext.Consumer>
                {state =>
                    <ContentHtmLRendererInner id={id} state={state} />
                }
            </EditorContext.Consumer>
        </div>
    );
};

const ImageUploadField = connectField(({ value, onChange }) => {
    return (
        <EditorContext.Consumer>
            {({ contentCache, contentRenderers, field }) => {
                const change: ((value: string | undefined) => void) = onChange as any;

                const select = () => {
                    const schemas = Object.keys(contentRenderers);

                    field?.pickContents(schemas, (contents) => {
                        if (contents.length > 0) {
                            const content = contents[0];

                            contentCache.set(content.id, content);
                            change(content.id);
                        }
                    });
                };

                const unselect = () => {
                    change(undefined);
                }

                return (
                    <div>
                        {!value ? (
                            <Button variant='contained' color='primary' onClick={select}>
                                <FilterIcon /> <span style={{ marginLeft: '.5rem' }}>Select Content</span>
                            </Button>
                        ) : (
                            <Button variant='contained' color='secondary' onClick={unselect}>
                                <ClearIcon />
                            </Button>
                        )}
                    </div>
                );
            }}
        </EditorContext.Consumer>
    );
});

export const squidexContent: CellPlugin<Data> = {
    Renderer: ContentHtmlRenderer,
    id: 'squidexContent',
    title: 'Squidex Content',
    description: 'Picks an content from Squidex',
    version: 1,
    controls: {
        type: 'autoform',
        schema: {
            properties: {
                id: {
                    type: 'string',
                    uniforms: {
                        label: 'ID',
                        component: ImageUploadField,
                    }
                }
            }
        },
    },
};