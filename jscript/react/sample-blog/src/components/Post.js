import React from 'react';
import { Link } from 'react-router-dom';

export const Post = ({ post, withLink }) => {
    return (
        <div>
            {withLink ? (
                <Link to={`/blog/${post.id}/${post.slug}`}>
                    <h2>{post.title}</h2>
                </Link>
            ) : (
                <h2>{post.title}</h2>
            )}

            <div dangerouslySetInnerHTML={{ __html: post.text }} />
        </div>
    );
};