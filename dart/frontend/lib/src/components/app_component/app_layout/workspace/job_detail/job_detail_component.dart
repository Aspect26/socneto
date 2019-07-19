import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/posts_list/posts_list_component.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';

import 'job_keywords_graph_component/job_keywords_graph_component.dart';

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
    JobKeywordsGraphComponent,
    PostsListComponent,
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

  final Router _router;
  final SocnetoService _socnetoService;

  Job job;
  List<Post> posts = [];

  JobDetailComponent(this._socnetoService, this._router);

  @override
  void onActivate(_, RouterState routerState) async {
    final parameters = routerState.parameters;
    final jobId = parameters[RouteParams.jobDetailJobId];
    this._setJob(jobId);
  }

  void _setJob(String jobId) async {
    try {
      this.job = await this._socnetoService.getJob(jobId);
      this.posts = await this._socnetoService.getJobPosts(jobId);
    } on HttpException catch (e) {
      this._onHttpError(e);
    }
  }

  void _onHttpError(HttpException exception) {
    if (exception.statusCode == 401) {
      this._router.navigate(RoutePaths.notAuthorized.toUrl());
    }

    throw exception;
  }

}