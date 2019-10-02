import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart_type_select/chart_type_select_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Success.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'create-chart-button',
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
    MaterialRadioComponent,
    MaterialRadioGroupComponent,
    MaterialInputComponent,
    AutoDismissDirective,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialTooltipDirective,
    MaterialDialogComponent,
    ModalComponent,
    ChartTypeSelectComponent,
    NgFor,
    NgIf,
  ],
  templateUrl: 'create_chart_button_component.html',
  styleUrls: ['create_chart_button_component.css'],
  providers: [materialProviders, overlayBindings],
)
class CreateChartButtonComponent {

  @Input() Job job;

  final SocnetoService _socnetoService;

  bool showCreateDialog = false;

  String errorMessage;
  String jsonPath = "";
  ChartType chartType;

  CreateChartButtonComponent(this._socnetoService);

  void onClick() {
    if (!this.showCreateDialog) {
      this.showCreateDialog = true;
      this._reset();
    }
  }

  void onCloseDialog() {
    this.showCreateDialog = false;
  }

  void onSubmit() async {
    if (!this.isDefinitionCorrect()) {
      this.errorMessage = "Incorrect chart definition";
      return;
    }

    Success response;
    try {
      response = await this._socnetoService.createJobChartDefinition(job.id, ChartDefinition(jsonPath, chartType));
    } on HttpException catch (e) {
      Toastr.error("Error", "Could not submit the chart definition: ${e.toString()}");
      return;
    }

    if (response.success) {
      Toastr.success("Success", "The chart was successfully created!");
    } else {
      Toastr.success("Error", "Could not create the chart :(");
    }

    this.showCreateDialog = false;
  }

  void onChartTypeSelected(ChartType chartType) {
    this.chartType = chartType;
  }

  void onJasonPathChange(String jsonPath) {
    this.jsonPath = jsonPath;
  }

  bool isDefinitionCorrect() {
    return this.chartType != null && this.jsonPath.isNotEmpty;
  }

  void _reset() {
    this.jsonPath = "";
    this.errorMessage = null;
  }

}