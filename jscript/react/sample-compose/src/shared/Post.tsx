import { Link } from 'react-router-dom';
import { flatten } from '../utils';
import { Markdown } from './Markdown';

export const Post = ({ post, withLink }: { post: any, withLink?: true }) => {
    const { id } = post;
    const postData = flatten(post);

    return (
        <div>
            {withLink ? (
                <Link to={`/blog/${id}/${postData.slug}`}>
                    <h2>{postData.title}</h2>
                </Link>
            ) : (
                <h2>{postData.title}</h2>
            )}

            <Markdown markdown={postData.text.text} references={postData.text.contents} />
        </div>
    );
};