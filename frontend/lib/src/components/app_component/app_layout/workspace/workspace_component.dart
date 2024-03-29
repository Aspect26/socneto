import 'package:angular/angular.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/job_detail_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_list/job_list_component.dart';


@Component(
  selector: 'workspace',
  templateUrl: 'workspace_component.html',
  styleUrls: ['workspace_component.css'],
  directives: [
    routerDirectives,
    JobListComponent,
    JobDetailComponent
  ],
  exports: [RoutePaths, Routes],
  encapsulation: ViewEncapsulation.None
)
class WorkspaceComponent implements OnActivate {

  String username;

  @override
  void onActivate(RouterState previous, RouterState current) {
    this.username = current.parameters[RouteParams.workspaceUserName];
  }

}