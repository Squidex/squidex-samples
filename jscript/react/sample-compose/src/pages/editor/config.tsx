import divider from '@react-page/plugins-divider';
import image from '@react-page/plugins-image';
import slate from '@react-page/plugins-slate';
import spacer from '@react-page/plugins-spacer';
import { Hotel, Destination } from './../../shared';
import { ContentTypes } from './editor-context';
import { squidexImage } from './SquidexImage';
import { squidexContent } from './SquidexContent';

export const contentTypes: ContentTypes = {
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

export const cellPlugins = [
    divider,
    image,
    slate(),
    spacer,
    squidexImage, 
    ...Object.entries(contentTypes).map(([schema, contentType]) => squidexContent(schema, contentType))
];