import 'package:angular_router/angular_router.dart';

import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/job_detail_component.template.dart' as job_detail_template;
import 'package:sw_project/src/components/app_component/app_layout/workspace/create_job/create_job_component.template.dart' as create_job_template;
import 'package:sw_project/src/components/app_component/app_layout/workspace/welcome/welcome_component.template.dart' as welcome_template;


class RoutePaths {
  static final home = RoutePath(path: '');
  static final createJob = RoutePath(path: 'create-job');
  static final jobDetail = RoutePath(path: 'job/:jobId');
}


class Routes {

  static final home = RouteDefinition(
    routePath: RoutePaths.home,
    component: welcome_template.WelcomeComponentNgFactory,
  );

  static final createJob = RouteDefinition(
    routePath: RoutePaths.createJob,
    component: create_job_template.CreateJobComponentNgFactory,
  );

  static final jobDetail = RouteDefinition(
    routePath: RoutePaths.jobDetail,
    component: job_detail_template.JobDetailComponentNgFactory,
  );

  static final all = <RouteDefinition>[home, createJob, jobDetail];

}