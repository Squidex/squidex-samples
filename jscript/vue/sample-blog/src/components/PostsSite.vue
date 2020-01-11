<template>
    <div v-if="posts">
        <div v-for="post in posts" v-bind:key="post.id">
            <Post v-bind:post="post" v-bind:withLink="true" />

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
    methods: {
        loadPosts: async function() {
            this.posts = null;

            try {
                const result = await getPosts();

                this.posts = result.posts;
            } catch (ex) {
                this.posts = [];
            }

        }
    },
    created: function() {
        this.loadPosts();
    }
}
</script>
