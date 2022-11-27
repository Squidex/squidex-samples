import { Button } from '@mui/material';
import ClearIcon from '@mui/icons-material/Clear';
import FilterIcon from '@mui/icons-material/Filter';
import { CellPlugin, CellPluginComponentProps, lazyLoad } from '@react-page/editor';
import { connectField, HTMLFieldProps } from 'uniforms';
import { ContentType, useEditorContext } from './editor-context';
import { useEffect, useState } from 'react';
import { CancellablePromise } from '../../utils';

type Data = {
    // The content ID.
    id?: string;
}

type SchemaProps = {
    // The name of the schema.
    schema: string;

    // The content type.
    contentType: ContentType;
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

const ContentHtmLRenderer = (props: CellPluginComponentProps<Data> & SchemaProps) => {
    const { contentType, data } = props;
    const { contentCache } = useEditorContext();
    const [contentItem, setContentItem] = useState<any>(undefined);

    useEffect(() => {
        const promise = new CancellablePromise(contentCache.get(data.id));

        promise.then(content => {
            setContentItem(content);
        });

        return () => {
            promise.cancel();
        };
    }, [data.id, contentCache]);

    if (contentItem) {
        const ContentRenderer = contentType.renderer;

        return (
            <ContentRenderer content={contentItem} />
        )
    } else {
        return (
            <div>
                <div className='react-page-plugins-content-image-placeholder'>
                    <ContentIcon style={iconStyle} />
                </div>
            </div>
        )
    }
};

type ContentPickerProps = HTMLFieldProps<string, HTMLDivElement, SchemaProps>;

const ContentPicker = connectField<ContentPickerProps>(props => {
    const { onChange, schema, value } = props;
    const { contentCache, field } = useEditorContext();

    const select = () => {
        field?.pickContents([schema], (contents) => {
            if (contents.length > 0) {
                const content = contents[0];

                contentCache.set(content.id, content);
                onChange(content.id);
            }
        });
    };

    const unselect = () => {
        onChange(undefined);
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
});

export function squidexContent(schema: string, contentType: ContentType): CellPlugin<Data> {
    return {
        id: `squidexContent_${schema}`,
        title: `Squidex ${contentType.name} Content`,
        description: `Picks an ${contentType.name} content from Squidex`,
        version: 1,
        controls: {
            type: 'autoform',
            schema: {
                properties: {
                    id: {
                        type: 'string',
                        uniforms: {
                            label: 'ID',
                            component: props => (
                                <ContentPicker {...props} schema={schema} contentType={contentType} />
                            ),
                        }
                    }
                }
            },
        },
        Renderer: props => (
            <ContentHtmLRenderer {...props} schema={schema} contentType={contentType} />
        ),
    }
}