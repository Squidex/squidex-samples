import { Button } from '@mui/material';
import ClearIcon from '@mui/icons-material/Clear';
import FilterIcon from '@mui/icons-material/Filter';
import { CellPlugin, CellPluginComponentProps, lazyLoad } from '@react-page/editor';
import { connectField, HTMLFieldProps } from 'uniforms';
import { useEditorContext } from './editor-context';

type Data = {
    // The asset URL.
    src?: string;

    // The alternate text.
    alt?: string;

    // The image target.
    openInNewWindow: boolean;

    // The optional URL.
    href?: string;
}

const ImageIcon = lazyLoad(() => import('@mui/icons-material/Landscape'));

export const iconStyle: React.CSSProperties = {
    width: '100%',
    height: 'auto',
    padding: '0',
    color: '#aaa',
    textAlign: 'center',
    minWidth: 64,
    minHeight: 64,
    maxHeight: 256,
};

const ImageHtmlRenderer = (props: CellPluginComponentProps<Data>) => {
    const { alt, href, openInNewWindow, src } = props.data;

    const image = (
        <img className='react-page-plugins-content-image' alt={alt} src={src} />
    );

    return src ? (
        <div>
            {href ? (
                <a onClick={props.isEditMode ? (e) => e.preventDefault() : undefined} href={href} target={openInNewWindow ? '_blank' : undefined} rel={openInNewWindow ? 'noreferrer noopener' : undefined}
                >
                    {image}
                </a>
            ) : (
                image
            )}
        </div>
    ) : (
        <div>
            <div className='react-page-plugins-content-image-placeholder'>
                <ImageIcon style={iconStyle} />
            </div>
        </div>
    );
};

type AssetPickerProps = HTMLFieldProps<string, HTMLDivElement>;

const AssetPicker =  connectField<AssetPickerProps>(props => {
    const { onChange, value } = props;
    const { field } = useEditorContext();

    const select = () => {
        field?.pickAssets((assets) => {
            if (assets.length > 0) {
                const context = field?.getContext();

                const assetObj = assets[0];
                const assetUrl = `${context.apiUrl}/assets/${context.appName}/${assetObj.id}/${assetObj.slug}`;

                onChange(assetUrl);
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
                    <FilterIcon /> <span style={{ marginLeft: '.5rem' }}>Select Asset</span> 
                </Button>
            ) : (
                <Button variant='contained' color='secondary' onClick={unselect}>
                    <ClearIcon />
                </Button>
            )}
        </div>
    );
});

export const squidexImage: CellPlugin<Data> = {
    id: 'squidexImage',
    title: 'Squidex Image',
    description: 'Picks an image from Squidex',
    version: 1,
    controls: {
        type: 'autoform',
        schema: {
            properties: {
                src: {
                    type: 'string',
                    uniforms: {
                        label: 'Source',
                        component: AssetPicker,
                    }
                },
                href: {
                    type: 'string',
                    uniforms: {
                        label: 'Link URL'
                    }
                },
                openInNewWindow: {
                    type: 'boolean',
                    uniforms: {
                        label: 'Open In New Window'
                    }
                },
                alt: {
                    type: 'string',
                    uniforms: {
                        label: 'Alternative Text'
                    }
                },
            }
        },
    },
    Renderer: ImageHtmlRenderer,
};