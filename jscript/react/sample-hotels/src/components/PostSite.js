import React from 'react';
import { useParams } from 'react-router-dom';
import { Post } from './Post.js';
import { getPost, getPostsIds } from './../service';
import { useRefresh } from './hooks.js';

export const PostSite = () => {
    const { id } = useParams();

    const [post, setPost] = React.useState(undefined);
    const needsUpdate = useRefresh(getPostsIds([post.id]));

    React.useEffect(() => {
        async function fetchData() {
            try {
                const result = await getPost(id);
        
                setPost(result);
            } catch (ex) {
                setPost(null);
            }
        }

        fetchData();
    }, [id, needsUpdate]);

    if (post) {
        return <Post post={post} />
    } else if (post === null) {
        return <div>Post not found.</div>
    } else {
        return <div>Loading Post...</div>
    }
}
