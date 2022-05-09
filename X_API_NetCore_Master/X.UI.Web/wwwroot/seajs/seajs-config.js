seajs && seajs.config({
    charset: "utf-8",
    timeout: 20000,
    debug: false,
    alias: {
        'jQuery': "/js/util/library/jquery",
        'json': "/js/util/library/json",
        'underscore': "/js/util/library/underscore",
        'plugins': "/js/util/plugins",
        'rsa': "/js/util/rsa/encrypt"
    }
});