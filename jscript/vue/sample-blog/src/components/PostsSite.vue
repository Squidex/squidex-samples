<template>
    <div v-if="posts">
        <div v-for="post in posts" v-bind:key="post.id">
            <Post v-bind:post="post" />

            <hr />
        </div>
    </div>
    <div v-else>
        Loading posts...
    </div>
</template>

<script>
import { getPosts } from './../service';
import Post from './Post.vue'

export default {
    name: 'PostsSite',
    components: {
        Post
    },
    data: () => {
        return {
            posts: null
        }
    },
    created: function() {
        const component = this;

        async function loadPosts() {
            component.posts = null;

            try {
                const result = await getPosts();

                component.posts = result.posts;
            } catch (ex) {
                component.posts = [];
            }
        }

        loadPosts();
    }
}
</script>
