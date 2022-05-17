<template>
    <main class="d-flex flex-column main-app">
        <!-- Begin page content -->
        <main class="flex-shrink-0">
            <div class="container">
                <h1 class="mt-5">Sticky footer</h1>
                <p class="lead">Pin a footer to the bottom of the viewport in desktop browsers with this custom HTML and CSS.</p>
                <p>Use <a href="../examples/sticky-footer-navbar/">the sticky footer with a fixed navbar</a> if need be, too.</p>
            </div>
        </main>

        <footer class="footer mt-auto py-3 bg-light">
            <div class="container">
                <span class="text-muted">Place sticky footer content here.</span>
            </div>
        </footer>
    </main>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        name: 'Index',
    });
</script>

<style scoped>
    @import '../core/css/type.css';
    @import '../core/css/api.css';
    @import '../css/base.css';
    @import './sticky-footer.css';
</style>
