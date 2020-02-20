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
import 'package:sw_project/src/components/shared/component_select/component_select_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/JobStatusCode.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';

import '../../../../../../utils.dart';


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
    ComponentSelectComponent,
    NgFor,
    NgIf,
  ],
  templateUrl: 'job_stats_component.html',
  styleUrls: ['job_stats_component.css'],
  providers: [
    materialProviders
  ],
)
class JobStatsComponent implements OnInit{

  final SocnetoService _socnetoService;
  final ChartDefinition postsFrequencyChart = ChartDefinition("Posts frequency", [], ChartType.PostsFrequency, true, id: "dummy1");
  final ChartDefinition languageFrequencyChart = ChartDefinition("Language frequency", [], ChartType.LanguageFrequency, false, id: "dummy2");

  @Input() Job job;
  int postsCount;
  List<SocnetoComponent> acquirers = [];
  List<SocnetoComponent> analyzers = [];
  bool loadingAcquirers = true;
  bool loadingAnalyzers = true;

  JobStatsComponent(this._socnetoService);

  String get runningTime {
    var fromTime = job.startedAt;
    var toTime = job.finishedAt != null? job.finishedAt : DateTime.now();
    var timeDiff = toTime.difference(fromTime);

    return getDurationString(timeDiff);
  }

  String get status {
    switch (job.status) {
      case JobStatusCode.Running: return "Running";
      case JobStatusCode.Stopped: return "Stopped";
    }
  }

  @override
  void ngOnInit() async {
    try {
      // TODO: nice.... getting paginated posts for count only
      var paginatedPosts = await this._socnetoService.getJobPosts(job.id, 1, 1, [], [], null);
      this.postsCount = paginatedPosts.paging.totalSize;
    } on HttpException catch (e) {
      Toastr.httpError(e);
    }
  }

}
