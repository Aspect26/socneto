import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart/chart_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/posts_list/posts_list_component.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'job-stats',
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
    PostsListComponent,
    ChartComponent,
    NgFor,
    NgIf,
  ],
  templateUrl: 'job_stats_component.html',
  styleUrls: ['job_stats_component.css'],
  providers: [
    materialProviders
  ],
)
class JobStatsComponent {

  final SocnetoService _socnetoService;
  final ChartDefinition postsFrequencyChart = ChartDefinition("Posts frequency", [], ChartType.PostsFrequency, true, id: "asd");

  @Input() Job job;

  JobStatsComponent(this._socnetoService);

}
