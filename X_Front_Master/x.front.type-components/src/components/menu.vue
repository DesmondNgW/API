<template>
    <nav class="navbar navbar-expand navbar-light bg-light" aria-label="Second navbar example">
        <div class="container-fluid">
            <router-link :to="{name:'index'}" class="navbar-brand">Index</router-link>
            <div class="collapse navbar-collapse" id="navbarsExample02">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <router-link :to="{name:'album'}" class="nav-link active">album</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link :to="{name:'navbars'}" class="nav-link">navbars</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link :to="{name:'navbars-static'}" class="nav-link">navbars-static</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link :to="{name:'navbars-fixed'}" class="nav-link">navbars-fixed</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link :to="{name:'navbars-bottom'}" class="nav-link">navbars-bottom</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link :to="{name:'blog'}" class="nav-link">blog</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link :to="{name:'carousel'}" class="nav-link">carousel</router-link>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</template>

<script lang="ts">
import Vue from 'vue';

export default Vue.extend({
  name: 'Menu',
});
</script>

<style>
    @import './core/css/nav.css';
    @import './core/css/navbar.css';
    @import './core/css/container.css';
</style>
