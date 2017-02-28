(function () {
    var gulp = require("gulp"),
        transport = require("gulp-cmd-transport"),
        nice = require("gulp-cmd-nice"),
        uglify = require("gulp-uglify"),
        importCss = require("gulp-import-css"),
        minifyCss = require("gulp-minify-css"),
        imageMin = require("gulp-imagemin"),
        revAll = require("gulp-rev-all"),
        filter = require("gulp-filter"),
        revOptions = new revAll({
            dontRenameFile: [".html", /sea\-(debug\-){0,1}[\d\.]+/],
            dontGlobal: [/^\/favicon.ico$/, ".bat", ".txt", ".config"],
            dontUpdateReference: [".html", /sea\-(debug\-){0,1}[\d\.]+/]
        }),
        buildTasks = [],
        destTasks = ["filter"],
        options = {
            build: "./__build/",
            dest: "../X.IO/Publish/Javascript/",
            filter: { js: filter("js/**/*.js", { restore: true }), config: filter("seajs/seajs-config.js", { restore: true }) }
        };
    ["*.config", "./css/**/*.css", "./html/**", "./images/**", "./js/**/*.js", "./lib/**", "./seajs/**"].forEach(function(src) {
        var id = src.split("*")[0].replace(/[\.\/]/g, ""), tid = id || "root", dest = /(^(lib){0,1}$)/.test(id) ? (destTasks.push(tid), options.dest + id) : (buildTasks.push(tid), options.build + id);
        switch (id) {
            case "images":
                return gulp.task(tid, function () { return gulp.src(src).pipe(imageMin({ optimizationLevel: 3, progressive: true, interlaced: true })).pipe(gulp.dest(dest)); });
            case "js":
                return gulp.task(tid, function () { return gulp.src(src).pipe(transport({ paths: ["."], debug: false, idleading: "/" })).pipe(gulp.dest(dest)); });
            case "css":
                return gulp.task(tid, function () { return gulp.src(src).pipe(importCss()).pipe(minifyCss()).pipe(gulp.dest(dest)); });
            default:
                return gulp.task(tid, function () { return gulp.src(src).pipe(gulp.dest(dest)); });
        }
    });
    gulp.task("filter", buildTasks, function () {
        return gulp.src(options.build + "**")
            .pipe(options.filter.js).pipe(nice.cmdConcat({ paths: ["."], include: "relative" })).pipe(uglify()).pipe(options.filter.js.restore)
            .pipe(options.filter.config).pipe(uglify()).pipe(options.filter.config.restore)
            .pipe(revOptions.revision()).pipe(gulp.dest(options.dest)).pipe(revOptions.manifestFile()).pipe(gulp.dest(options.dest));
    });
    gulp.task("default", destTasks, function () { return require("del")([options.build]); });
})();