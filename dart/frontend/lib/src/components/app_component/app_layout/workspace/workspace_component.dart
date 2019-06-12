import 'package:angular/angular.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/create_job/create_job_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/job_detail_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/jobs_list/jobs_list_component.dart';


@Component(
  selector: 'workspace',
  templateUrl: 'workspace_component.html',
  styleUrls: ['workspace_component.css'],
  directives: [
    routerDirectives,

    JobsListComponent,
    JobDetailComponent,
    CreateJobComponent,
  ],
  exports: [RoutePaths, Routes],
)
class WorkspaceComponent implements OnActivate {

  int userId;

  @override
  void onActivate(RouterState previous, RouterState current) {
    this.userId = int.parse(current.parameters["userId"]);
  }

}