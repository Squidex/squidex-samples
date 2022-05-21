import React from 'react';
import ReactMarkdown from 'react-markdown'
import { EmbeddableHotel } from './EmbeddableHotel';
import { CONFIG } from './../service';

const hotelsRegex = new RegExp(`${escapeRegExp(CONFIG.url)}\\/api/content\\/${escapeRegExp(CONFIG.appName)}\\/hotels/(?<id>[a-z0-9\\-]+)`);

export const Markdown = ({ markdown, references }) => {
    return (
        <ReactMarkdown children={markdown} components={{
            a({ href, children }) {
                const match = hotelsRegex.exec(href);

                if (match && match.groups) {
                    const referenceId = match.groups.id;
                    const reference = references?.find(x => x.id === referenceId);

                    if (reference) {
                        return <EmbeddableHotel hotel={reference} />;
                    }
                } 
                
                return <a href={href} target='_blank' rel='noopener noreferrer'>{children}</a>;
            }
        }} />
    )
};

function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
}