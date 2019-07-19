import 'package:angular_router/angular_router.dart';

import 'package:sw_project/src/components/app_component/app_layout/workspace/workspace_component.template.dart' as workspace_template;
import 'package:sw_project/src/components/app_component/app_layout/login/login_component.template.dart' as login_template;

import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/job_detail_component.template.dart' as job_detail_template;
import 'package:sw_project/src/components/app_component/app_layout/workspace/create_job/create_job_component.template.dart' as create_job_template;
import 'package:sw_project/src/components/app_component/app_layout/workspace/welcome/welcome_component.template.dart' as welcome_template;


class RoutePaths {
  static final login = RoutePath(path: 'login');
  static final workspace = RoutePath(path: 'workspace/:${RouteParams.workspaceUserId}');

  static final workspaceHome = RoutePath(path: '', parent: RoutePaths.workspace);
  static final createJob = RoutePath(path: 'create-job', parent: RoutePaths.workspace);
  static final jobDetail = RoutePath(path: 'job/:${RouteParams.jobDetailJobId}', parent: RoutePaths.workspace);
}

class RouteParams {
  static final workspaceUserId = "userId";
  static final jobDetailJobId = "jobId";

  static Map<String, String> workspaceParams(int userId) => {
    workspaceUserId: "$userId"
  };

  static Map<String, String> jobDetailParams(int userId, String jobId) => {
    workspaceUserId: "$userId",
    jobDetailJobId: jobId
  };
}


class Routes {

  static final login = RouteDefinition(
    routePath: RoutePaths.login,
    component: login_template.LoginComponentNgFactory,
    useAsDefault: true,
  );

  static final workspace = RouteDefinition(
    routePath: RoutePaths.workspace,
    component: workspace_template.WorkspaceComponentNgFactory
  );

  static final workspaceHome = RouteDefinition(
    routePath: RoutePaths.workspaceHome,
    component: welcome_template.WelcomeComponentNgFactory,
    useAsDefault: true,
  );

  static final createJob = RouteDefinition(
    routePath: RoutePaths.createJob,
    component: create_job_template.CreateJobComponentNgFactory,
  );

  static final jobDetail = RouteDefinition(
    routePath: RoutePaths.jobDetail,
    component: job_detail_template.JobDetailComponentNgFactory,
  );

  static final workspaceRoutes = <RouteDefinition>[workspaceHome, createJob, jobDetail];
  static final appRoutes = <RouteDefinition>[login, workspace];

}