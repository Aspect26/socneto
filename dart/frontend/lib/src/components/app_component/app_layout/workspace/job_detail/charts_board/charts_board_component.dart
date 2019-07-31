import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/create_chart_button_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'charts-board',
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

    CreateChartButtonComponent,
    ChartComponent,

    NgFor,
    NgIf,
  ],
  templateUrl: 'charts_board_component.html',
  styleUrls: ['charts_board_component.css'],
  providers: [materialProviders],
)
class ChartsBoardComponent implements AfterChanges {

  @Input() Job job;

  List<ChartDefinition> chartDefinitions = [];
  List<AnalyzedPost> analyzedPosts = [];

  SocnetoService _socnetoService;

  ChartsBoardComponent(this._socnetoService);

  @override
  void ngAfterChanges() async {
    try {
      this.analyzedPosts = await this._socnetoService.getJobAnalysis(this.job.id);
    } catch (e) {
      // TODO: HttpException when merged
      Toastr.error("Posts", "Could not fetch analyzed posts for this job: ${e.toString()}");
    }

    try {
      this.chartDefinitions = await this._socnetoService.getJobChartDefinitions(this.job.id);
    } catch(e) {
      // TODO: HttpException when merged
      Toastr.error("Charts", "Could not fetch charts for this job: ${e.toString()}");
    }

  }

}