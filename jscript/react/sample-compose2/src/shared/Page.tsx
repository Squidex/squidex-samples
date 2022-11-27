import { flatten } from '../utils';
import { Markdown } from './Markdown';

export const Page = ({ page }: { page: any }) => {
    const pageData = flatten(page);

    return (
        <div>
            <h2>{pageData.title}</h2>

            <Markdown markdown={pageData.text.text} references={pageData.text.contents} />
        </div>
    );
};