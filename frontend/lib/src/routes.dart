import 'package:angular_router/angular_router.dart';

import 'package:sw_project/src/components/app_component/app_layout/workspace/workspace_component.template.dart' as workspace_template;
import 'package:sw_project/src/components/app_component/app_layout/login/login_component.template.dart' as login_template;
import 'package:sw_project/src/components/app_component/app_layout/not_authorized/not_authorized_component.template.dart' as not_authorized_template;

import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/job_detail_component.template.dart' as job_detail_template;
import 'package:sw_project/src/components/app_component/app_layout/workspace/quick_guide/quick_guide_component.template.dart' as quick_guide_template;


class RoutePaths {
  static final login = RoutePath(path: 'login');
  static final workspace = RoutePath(path: 'workspace/:${RouteParams.workspaceUserName}');
  static final notAuthorized = RoutePath(path: 'not-authorized');

  static final workspaceHome = RoutePath(path: '', parent: RoutePaths.workspace);
  static final jobDetail = RoutePath(path: 'job/:${RouteParams.jobDetailJobId}', parent: RoutePaths.workspace);
}

class RouteParams {
  static final workspaceUserName = "userName";
  static final jobDetailJobId = "jobId";

  static Map<String, String> workspaceParams(String username) => {
    workspaceUserName: "$username"
  };

  static Map<String, String> jobDetailParams(String username, String jobId) => {
    workspaceUserName: "$username",
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

  static final notAuthorized = RouteDefinition(
    routePath: RoutePaths.notAuthorized,
    component: not_authorized_template.NotAuthorizedComponentNgFactory
  );

  static final workspaceHome = RouteDefinition(
    routePath: RoutePaths.workspaceHome,
    component: quick_guide_template.QuickGuideComponentNgFactory,
    useAsDefault: true,
  );

  static final jobDetail = RouteDefinition(
    routePath: RoutePaths.jobDetail,
    component: job_detail_template.JobDetailComponentNgFactory,
  );

  static final workspaceRoutes = <RouteDefinition>[workspaceHome, jobDetail];
  static final appRoutes = <RouteDefinition>[login, workspace, notAuthorized];

}