import 'package:angular/angular.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/components/posts_list/posts_list_component.dart';
import 'package:sw_project/src/components/job_graphs/job_graphs_component.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/services/socneto_service.dart';

@Component(
  selector: 'job-detail',
  directives: [
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    NgFor,
    NgIf,
    PostsListComponent,
    TaskGraphsComponent
  ],
  templateUrl: 'job_detail_component.html',
  styleUrls: ['job_detail_component.css'],
  providers: [
    ClassProvider(SocnetoService)
  ],
)
class JobDetailComponent {

  Job task;
  List<Post> posts = [];

  SocnetoService _socnetoService;

  JobDetailComponent(this._socnetoService);

  void setTask(Job task) async {
    this.task = task;
    this.posts = await this._socnetoService.getJobPosts(task.id);
  }

}