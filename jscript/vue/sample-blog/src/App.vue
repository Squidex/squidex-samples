<template>
    <div>
        <nav class="navbar fixed-top navbar-dark bg-primary navbar-expand-sm">
            <div class="container">
                <a href="/" class="navbar-brand">Squidex Blog Sample</a>

                <TopNav />
            </div>
        </nav>

        <div class="container body-content">
            <router-view></router-view>
        </div>
    </div>
</template>

<script>
import TopNav from "./components/TopNav.vue"

export default {
    name: "app",
    components: {
        TopNav
    }
}
</script>

<style>
    body {
        padding-top: 100px;
    }
</style>
