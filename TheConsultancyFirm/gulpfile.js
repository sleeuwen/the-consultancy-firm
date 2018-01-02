/// <binding BeforeBuild='min' Clean='clean' ProjectOpened='watch' />
"use strict";

var gulp = require("gulp"),
    autoprefixer = require("gulp-autoprefixer"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    plumber = require("gulp-plumber"),
    sass = require("gulp-sass"),
    sourcemaps = require("gulp-sourcemaps"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json");

var regex = {
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

gulp.task("sass", function () {
    return gulp.src("Styles/site.scss")
        .pipe(plumber())
        .pipe(sourcemaps.init())
        .pipe(sass().on("error", sass.logError))
        .pipe(autoprefixer())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("wwwroot/css"));
});
gulp.task("sass:dashboard", function () {
    return gulp.src("Styles/dashboard.scss")
        .pipe(plumber())
        .pipe(sourcemaps.init())
        .pipe(sass().on("error", sass.logError))
        .pipe(autoprefixer())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("wwwroot/css"));
});
gulp.task("sass:login", function () {
    return gulp.src("Styles/login.scss")
        .pipe(plumber())
        .pipe(sourcemaps.init())
        .pipe(sass().on("error", sass.logError))
        .pipe(autoprefixer())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("wwwroot/css"));
});
gulp.task("sass:tinymce", function () {
    return gulp.src("Styles/tinymce.scss")
        .pipe(plumber())
        .pipe(sourcemaps.init())
        .pipe(sass().on("error", sass.logError))
        .pipe(autoprefixer())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest("wwwroot/css"));
});

gulp.task("min", ["min:js", "min:css", "min:html"]);

gulp.task("min:js", function () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(plumber())
            .pipe(concat(bundle.outputFileName))
            .pipe(uglify())
            .pipe(gulp.dest("."));
    });
    if (tasks.length === 0) return;
    return merge(tasks);
});

gulp.task("min:css", ["sass", "sass:dashboard", "sass:login", "sass:tinymce"], function () {
    var tasks = getBundles(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(plumber())
            .pipe(concat(bundle.outputFileName))
            .pipe(cssmin())
            .pipe(gulp.dest("."));
    });
    if (tasks.length === 0) return;
    return merge(tasks);
});

gulp.task("min:html", function () {
    var tasks = getBundles(regex.html).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(plumber())
            .pipe(concat(bundle.outputFileName))
            .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
            .pipe(gulp.dest("."));
    });
    if (tasks.length === 0) return;
    return merge(tasks);
});

gulp.task("clean", function () {
    var files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });

    return del(files);
});

gulp.task("watch", ["min"], function () {
    gulp.watch("Styles/**/*.scss", ["min:css"]);

    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:js"]);
    });

    // getBundles(regex.css).forEach(function (bundle) {
    //     gulp.watch(bundle.inputFiles, ["min:css"]);
    // });

    getBundles(regex.html).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:html"]);
    });
});

gulp.task("default", ["sass", "watch"]);

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}
