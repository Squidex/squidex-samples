<template>
    <div>
        <nav class="navbar fixed-top navbar-dark bg-primary navbar-expand-sm">
            <div class="container">
                <a href="/" class="navbar-brand">Squidex Blog Sample</a>

                <TopNav />
            </div>
        </nav>

        <div class="container body-content">
            <PostsSite />
        </div>
    </div>
</template>

<script>
import PostsSite from "./components/PostsSite.vue"
import TopNav from "./components/TopNav.vue"

export default {
    name: "app",
    components: {
        PostsSite,
        TopNav
    }
}
</script>

<style>
    body {
        padding-top: 100px;
    }
</style>
