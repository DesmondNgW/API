<template>
    <main class="d-flex flex-column main-app">
        <header>
            <!-- Fixed navbar -->
            <nav class="navbar navbar-expand-md navbar-dark fixed-top bg-dark">
                <div class="container-fluid">
                    <a class="navbar-brand" href="#">Fixed navbar</a>
                    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarCollapse" aria-controls="navbarCollapse" aria-expanded="false" aria-label="Toggle navigation">
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse" id="navbarCollapse">
                        <ul class="navbar-nav me-auto mb-2 mb-md-0">
                            <li class="nav-item">
                                <a class="nav-link active" aria-current="page" href="#">Home</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#">Link</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link disabled">Disabled</a>
                            </li>
                        </ul>
                        <form class="d-flex">
                            <input class="form-control me-2" type="search" placeholder="Search" aria-label="Search">
                            <button class="btn btn-outline-success" type="submit">Search</button>
                        </form>
                    </div>
                </div>
            </nav>
        </header>

        <!-- Begin page content -->
        <main class="flex-shrink-0">
            <div class="container">
                <h1 class="mt-5">Sticky footer with fixed navbar</h1>
                <p class="lead">Pin a footer to the bottom of the viewport in desktop browsers with this custom HTML and CSS. A fixed navbar has been added with <code class="small">padding-top: 60px;</code> on the <code class="small">main &gt; .container</code>.</p>
                <p>Back to <a href="../examples/sticky-footer/">the default sticky footer</a> minus the navbar.</p>
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
    @import '../core/css/forms/control.css';
    @import '../core/css/button.css';
    @import '../core/css/type.css';
    @import '../core/css/api.css';
    @import '../css/base.css';
    nav.fixed-top {
        top: 5.8rem;
    }
</style>
<style src="./sticky-footer-navbar.css" scoped></style>