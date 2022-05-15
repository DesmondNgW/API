<template>
    <div id="app">
        <Menu></Menu>
        <router-view />
    </div>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Menu from './components/menu.vue';
    export default Vue.extend({
        name: 'App',
        components: { Menu }
    });
</script>

<style>
    @import './components/core/css/root.css';
    @import './components/core/css/reboot.css';
</style>
