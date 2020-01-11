import Vue from 'vue'
import VueRouter from 'vue-router'
import PageSite from './../components/PageSite';
import PostSite from './../components/PostSite';
import PostsSite from './../components/PostsSite';

Vue.use(VueRouter)

const routes = [
    {
        path: '/',
        name: 'home',
        component: PostsSite
    },
    {
        path: '/pages/:slug',
        name: 'page',
        component: PageSite
    },
    {
        path: '/blog/:id/:slug',
        name: 'blog',
        component: PostSite
    },
    {
        path: '*',
        name: 'fallback',
        redirect: { name: 'home' }
    }
]

const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes
})

export default router
