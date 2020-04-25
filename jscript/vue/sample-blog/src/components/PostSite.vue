<template>
    <div v-if="post">
        <Post v-bind:post="post" />
    </div>
    <div v-else-if="post === null">
        Post not found
    </div>
    <div v-else>
        Loading post...
    </div>
</template>

<script>
import { getPost } from './../service';
import Post from './Post.vue'

export default {
    name: 'PostsSite',
    components: {
        Post
    },
    data: () => {
        return {
            post: null
        }
    },
    methods: {
        loadPost: async function(id) {
            this.post = undefined;

            try {
                const result = await getPost(id);

                this.post = result;
            } catch (ex) {
                this.post = null;
            }
        }
    },
    created: function() {
        this.loadPost(this.$route.params.id)
    },
    watch: {
        $route: {
            handler: function(to) {
                this.loadPost(to.params.id);
            }
        }
    }
}
</script>
