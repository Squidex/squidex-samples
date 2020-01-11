<template>
    <div>
        <h2>{{post.title}}</h2>

        <div v-html="post.text"></div>
    </div>
</template>

<script>
export default {
    name: 'Post',
    props: {
        post: Object
    }
}
</script>
