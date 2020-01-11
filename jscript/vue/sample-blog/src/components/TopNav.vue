<template>
    <ul class="navbar-nav mr-auto">
        <li v-for="page in pages" v-bind:key="page.id" class="nav-item">
            <router-link :to="{ name: 'page', params: { slug: page.slug }}" class="nav-link">{{page.title}}</router-link>
        </li>
    </ul>
</template>

<script>
import { getPages } from "./../service";

export default {
    name: "TopNav",
    data: () => {
        return {
            pages: null
        }
    },
    created: function() {
        const component = this;

        async function loadPages() {
            try {
                const result = await getPages();

                component.pages = result.pages;
            } catch (ex) {
                component.pages = [];
            }
        }

        loadPages();
    }
}
</script>

<style scoped>
    .nav-link.router-link-active {
        color: white !important;
    }
</style>