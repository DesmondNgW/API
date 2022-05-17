import Vue from "vue";
import Router from "vue-router";
import Index from "../components/index.vue";
Vue.use(Router);
export default new Router({
    routes: [
        {
            path: "/",
            name: "index",
            component: Index
        },
        {
            path: "/album/index",
            name: "album",
            component: () => import("../components/album/index.vue")
        },
        {
            path: "/navbars/index",
            name: "navbars",
            component: () => import("../components/navbars/index.vue")
        },
        {
            path: "/navbars-static/index",
            name: "navbars-static",
            component: () => import("../components/navbars-static/index.vue")
        },
        {
            path: "/navbars-fixed/index",
            name: "navbars-fixed",
            component: () => import("../components/navbars-fixed/index.vue")
        },
        {
            path: "/navbars-bottom/index",
            name: "navbars-bottom",
            component: () => import("../components/navbars-bottom/index.vue")
        },
        {
            path: "/blog/index",
            name: "blog",
            component: () => import("../components/blog/index.vue")
        },
        {
            path: "/carousel/index",
            name: "carousel",
            component: () => import("../components/carousel/index.vue")
        },
        {
            path: "/cheatsheet/index",
            name: "cheatsheet",
            component: () => import("../components/cheatsheet/index.vue")
        },
        {
            path: "/checkout/index",
            name: "checkout",
            component: () => import("../components/checkout/index.vue")
        },
        {
            path: "/cover/index",
            name: "cover",
            component: () => import("../components/cover/index.vue")
        },
        {
            path: "/dropdowns/index",
            name: "dropdowns",
            component: () => import("../components/dropdowns/index.vue")
        },
        {
            path: "/features/index",
            name: "features",
            component: () => import("../components/features/index.vue")
        },
        {
            path: "/footers/index",
            name: "footers",
            component: () => import("../components/footers/index.vue")
        },
        {
            path: "/grid/index",
            name: "grid",
            component: () => import("../components/grid/index.vue")
        },
        {
            path: "/headers/index",
            name: "headers",
            component: () => import("../components/headers/index.vue")
        },
        {
            path: "/heroes/index",
            name: "heroes",
            component: () => import("../components/heroes/index.vue")
        },
        {
            path: "/jumbotron/index",
            name: "jumbotron",
            component: () => import("../components/jumbotron/index.vue")
        },
        {
            path: "/list-groups/index",
            name: "list-groups",
            component: () => import("../components/list-groups/index.vue")
        },
        {
            path: "/masonry/index",
            name: "masonry",
            component: () => import("../components/masonry/index.vue")
        },
        {
            path: "/modals/index",
            name: "modals",
            component: () => import("../components/modals/index.vue")
        },
        {
            path: "/offcanvas-navbar/index",
            name: "offcanvas-navbar",
            component: () => import("../components/offcanvas-navbar/index.vue")
        },
        {
            path: "/pricing/index",
            name: "pricing",
            component: () => import("../components/pricing/index.vue")
        },
        {
            path: "/product/index",
            name: "product",
            component: () => import("../components/product/index.vue")
        },
        {
            path: "/sidebars/index",
            name: "sidebars",
            component: () => import("../components/sidebars/index.vue")
        },
        {
            path: "/sign-in/index",
            name: "sign-in",
            component: () => import("../components/sign-in/index.vue")
        },
        {
            path: "/starter-template/index",
            name: "starter-template",
            component: () => import("../components/starter-template/index.vue")
        },
        {
            path: "/sticky-footer/index",
            name: "sticky-footer",
            component: () => import("../components/sticky-footer/index.vue")
        },
        {
            path: "/sticky-footer-navbar/index",
            name: "sticky-footer-navbar",
            component: () => import("../components/sticky-footer-navbar/index.vue")
        }
    ]
});
