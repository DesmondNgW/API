<template>
    <nav class="navbar navbar-expand navbar-light bg-light" aria-label="Second navbar example">
        <div class="container-fluid">
            <router-link :to="{name:'index'}" class="navbar-brand">Icons</router-link>
            <div class="collapse navbar-collapse" id="navbarsExample02">
                <ul class="navbar-nav me-auto" style="flex-wrap:wrap">
                    <li class="nav-item" v-for="(item, i) in config" :key="i">
                        <router-link :to="{name: item.name}" class="nav-link active">{{item.name}}</router-link>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</template>

<script lang="ts">
    import Vue from 'vue';
    import router from '../router';
    export default Vue.extend({
        name: 'Menu',
        data() {
            let config = [];
            let data = router.options.routes;
            if (data) {
                for (let i = 0, len = data.length; i < len; i++) {
                    if (data[i].name == "index")  continue;
                    config.push(data[i]);
                }
            }
            return {
                config: config
            }
        }
    });
</script>

<style>
    @import './core/css/nav.css';
    @import './core/css/navbar.css';
    @import './core/css/container.css';
    @import './core/css/api.css';
</style>
