import { useMemo } from 'react';
import ReactMarkdown from 'react-markdown'
import { useApiContext } from '../utils';
import { EmbeddableHotel } from './EmbeddableHotel';

export const Markdown = ({ markdown, references }: { markdown: string, references?: any[] }) => {
    const api = useApiContext();

    const hotelsRegex = useMemo(() => {
        const { apiUrl, appName } = api;

        return new RegExp(`${escapeRegExp(apiUrl)}\\/api/content\\/${escapeRegExp(appName)}\\/hotels/(?<id>[a-z0-9\\-]+)`)
    }, [api]);

    return (
        <ReactMarkdown children={markdown} components={{
            a({ href, children }) {
                const match = hotelsRegex.exec(href || '');

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

function escapeRegExp(string: string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
}