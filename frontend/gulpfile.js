let gulp = require('gulp');
let concat = require('gulp-concat');


let socnetoJS = [
  'lib/src/charts/**/*.js',
];

let socnetoCSS = [
  'lib/src/charts/**/*.css',
];

gulp.task('makeOneFileToRuleThemAll', function(){
  return gulp.src(socnetoJS)
      .pipe(concat('socneto.js'))
      .pipe(gulp.dest('web/static/js'));
});

gulp.task('bundleCSS', function(){
  return gulp.src(socnetoCSS)
      .pipe(concat('socneto.css'))
      .pipe(gulp.dest('web/static/css'));
});

gulp.task('default', gulp.parallel('bundleCSS', 'makeOneFileToRuleThemAll'));
