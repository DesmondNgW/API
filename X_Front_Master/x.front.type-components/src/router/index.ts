import Vue from "vue";
import Router from "vue-router";
import Index from "../components/index.vue";
Vue.use(Router);
export default new Router({
    routes: [{
        path: "/",
        name: "index",
        component: Index
    }, {
        path: "/album/index",
        name: "album",
        component: () => import("../components/album/index.vue")
    }, {
        path: "/navbars/index",
        name: "navbars",
        component: () => import("../components/navbars/index.vue")
    }, {
        path: "/navbars-static/index",
        name: "navbars-static",
        component: () => import("../components/navbars-static/index.vue")
    }, {
        path: "/navbars-fixed/index",
        name: "navbars-fixed",
        component: () => import("../components/navbars-fixed/index.vue")
    }, {
        path: "/navbars-bottom/index",
        name: "navbars-bottom",
        component: () => import("../components/navbars-bottom/index.vue")
    }, {
        path: "/blog/index",
        name: "blog",
        component: () => import("../components/blog/index.vue")
    }, {
        path: "/carousel/index",
        name: "carousel",
        component: () => import("../components/carousel/index.vue")
    },]
});
