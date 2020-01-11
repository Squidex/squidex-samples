<template>
    <div v-if="page">
        <Page v-bind:page="page" />
    </div>
    <div v-else-if="page === null">
        Page not found
    </div>
    <div v-else>
        Loading page...
    </div>
</template>

<script>
import { getPage } from './../service';
import Page from './Page.vue'

export default {
    name: 'PostsSite',
    components: {
        Page
    },
    data: () => {
        return {
            page: null
        }
    },
    methods: {
        loadPage: async function(slug) {
            this.page = undefined;

            try {
                const result = await getPage(slug);

                this.page = result;
            } catch (ex) {
                this.page = null;
            }
        }
    },
    created: function() {
        this.loadPage(this.$route.params.slug)
    },
    watch: {
        $route: {
            handler: function(to) {
                this.loadPage(to.params.slug);
            }
        }
    }
}
</script>
