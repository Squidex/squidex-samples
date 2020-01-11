<template>
    <div>
        <div v-if="withLink">
            <router-link :to="{ name: 'post', params: { slug: post.slug, id: post.id }}">
                <h2>{{post.title}}</h2>
            </router-link>
        </div>
        <div v-else>
            <h2>{{post.title}}</h2>
        </div>

        <div v-html="post.text"></div>
    </div>
</template>

<script>
export default {
    name: 'Post',
    props: {
        post: Object,
        withLink: Boolean
    }
}
</script>
