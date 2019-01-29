var gulp = require('gulp');
var uglify = require("gulp-uglify");
var contact = require("gulp-contact");

gulp.task("minify", function () {
    return gulp.src("wwwroot/js/**/*js")
        .pipe(uglify())
        .pipe(concat("dutchtreat.min.js"))
        .pipe(gulp.dest("wwwroot/dist"));
});

gulp.task('default', ["minify"]);