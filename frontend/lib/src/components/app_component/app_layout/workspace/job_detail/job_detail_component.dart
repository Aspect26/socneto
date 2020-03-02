import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/charts_board_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/job_stats/job_stats_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/posts_list/posts_list_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'job-detail',
  directives: [
    DeferredContentDirective,
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    MaterialTabPanelComponent,
    MaterialTabComponent,
    ChartsBoardComponent,
    PostsListComponent,
    JobStatsComponent,
    NgFor,
    NgIf,
  ],
  templateUrl: 'job_detail_component.html',
  styleUrls: ['job_detail_component.css'],
  providers: [
    materialProviders
  ],
)
class JobDetailComponent implements OnActivate {

  @ViewChild(ChartsBoardComponent) ChartsBoardComponent chartsBoardComponent;
  @ViewChild(JobStatsComponent) JobStatsComponent jobStatsComponent;

  final Router _router;
  final SocnetoService _socnetoService;

  Job job;

  JobDetailComponent(this._socnetoService, this._router);

  @override
  void onActivate(_, RouterState routerState) async {
    final parameters = routerState.parameters;
    final jobId = parameters[RouteParams.jobDetailJobId];
    this._setJob(jobId);
  }

  void onTabChange(TabChangeEvent event) {
    if (event.newIndex == 0) {
      this.jobStatsComponent.refreshCharts();
    } else if (event.newIndex == 1) {
      this.chartsBoardComponent.refreshCharts();
    }
  }

  void _setJob(String jobId) async {
    try {
      this.job = await this._socnetoService.getJob(jobId);
    } on NotAuthorizedException {
      await this._router.navigate(RoutePaths.notAuthorized.toUrl());
    } on HttpException catch (e) {
      Toastr.httpError(e);
    }
  }

}
