<template>
    <main class="main-app text-center">
        <main class="form-signin">
            <form>
                <h1 class="h3 mb-3 fw-normal">Please sign in</h1>

                <div class="form-floating">
                    <input type="email" class="form-control" id="floatingInput" placeholder="name@example.com">
                    <label for="floatingInput">Email address</label>
                </div>
                <div class="form-floating">
                    <input type="password" class="form-control" id="floatingPassword" placeholder="Password">
                    <label for="floatingPassword">Password</label>
                </div>

                <div class="checkbox mb-3">
                    <label>
                        <input type="checkbox" value="remember-me"> Remember me
                    </label>
                </div>
                <button class="w-100 btn btn-lg btn-primary" type="submit">Sign in</button>
                <p class="mt-5 mb-3 text-muted">&copy; 2017–2021</p>
            </form>
        </main>
    </main>
</template>

<script lang="ts">
    import Vue from 'vue';

    export default Vue.extend({
        name: 'Index',
    });
</script>

<style scoped>
    @import '../core/css/forms/floating.css';
    @import '../core/css/forms/check.css';
    @import '../core/css/forms/control.css';
    @import '../core/css/button.css';
    @import '../core/css/api.css';
    @import '../css/base.css';
    @import './signin.css';
</style>
