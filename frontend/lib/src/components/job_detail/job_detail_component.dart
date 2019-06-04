import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/components/job_keywords_graph_component/job_keywords_graph_component.dart';
import 'package:sw_project/src/components/posts_list/posts_list_component.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
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
    JobKeywordsGraphComponent,
    PostsListComponent,
    NgFor,
    NgIf,
  ],
  templateUrl: 'job_detail_component.html',
  styleUrls: ['job_detail_component.css'],
  providers: [
    ClassProvider(SocnetoService),
    materialProviders
  ],
)
class JobDetailComponent implements OnActivate {

  Job job;
  List<Post> posts = [];
  SocnetoService _socnetoService;

  JobDetailComponent(this._socnetoService);

  @override
  void onActivate(_, RouterState routerState) async {
    final parameters = routerState.parameters;
    // TODO: put this 'jonId' to constants somewhere (routes.dart possibly). Its also used elsewhere.
    final jobId = parameters["jobId"];
    this._setJob(jobId);
  }

  void _setJob(String jobId) async {
    this.job = await this._socnetoService.getJob(jobId);
    this.posts = await this._socnetoService.getJobPosts(jobId);
  }

}