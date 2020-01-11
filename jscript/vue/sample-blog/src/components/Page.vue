<template>
    <div>
        <h2>{page.title}</h2>

        <div v-html="page.text"></div>
    </div>
</template>

<script>
export default {
    name: 'Page',
    props: {
        page: Object
    }
}
</script>
