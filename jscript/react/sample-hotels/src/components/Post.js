import React from 'react';
import { Link } from 'react-router-dom';
import { Markdown } from './Markdown';

export const Post = ({ post, withLink }) => {
    const { id, flatData: data } = post;

    return (
        <div>
            {withLink ? (
                <Link to={`/blog/${id}/${data.slug}`}>
                    <h2>{data.title}</h2>
                </Link>
            ) : (
                <h2>{data.title}</h2>
            )}

            <Markdown markdown={data.text.text} references={data.text.contents} />
        </div>
    );
};