(function (global) {
    var envs = {
        /**
         * 本地
         */
        LOCAL: -1,
        /**
         * 开发环境
         */
        DEV: 0,
        /**
         * 测试环境
         */
        TESTING: 1,
        /**
         * 实盘环境
         */
        STAGING: 2,
        /**
         * 生产环境
         */
        PRODUCTION: 3,
        /**
         * 仿真
         */
        SIMULATION: 4,
    }
    var env = envs.LOCAL, config = null;
    switch (env) {
        case envs.LOCAL:
            config = {

            }
            break;
        case envs.DEV:
            config = {

            }
            break;
        case envs.TESTING:
            config = {

            }
            break;
        case envs.STAGING:
            config = {

            }
            break;
        case envs.PRODUCTION:
            config = {

            }
            break;
        case envs.SIMULATION:
            config = {

            }
            break;
    }
    global.envConfig = config;
}).call(this, window);
