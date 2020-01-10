import React from 'react';

import { Post } from './Post.js';
import { getPosts } from './../service';

export const PostsSite = () => {
    const [posts, setPosts] = React.useState();

    React.useEffect(() => {
        async function fetchData() {
            const results = await getPosts();
    
            setPosts(results.posts);
        }

        fetchData();
    }, []);

    if (!posts) {
        return <div>Loading posts...</div>
    }

    return posts.map(post => (
        <div key={post.id}>
            <Post key={post.id} post={post} useLink={true} />

            <hr />
        </div>
    ));
}
