import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart_type_select/chart_type_select_component.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/create_chart_modal_component.dart';
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
    MaterialDropdownSelectComponent,
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
    MaterialFabComponent,
    ModalComponent,
    ChartTypeSelectComponent,
    CreateChartModalComponent,
    NgFor,
    NgIf,
  ],
  templateUrl: 'create_chart_button_component.html',
  styleUrls: ['create_chart_button_component.css'],
  providers: [materialProviders, overlayBindings],
)
class CreateChartButtonComponent {

  @Input() Job job;
  @ViewChild(CreateChartModalComponent) CreateChartModalComponent createChartModal;

  final SocnetoService _socnetoService;

  bool showCreateDialog = false;

  CreateChartButtonComponent(this._socnetoService);

  void onClick() {
    if (!this.createChartModal.isShown()) {
      this.createChartModal.show();
    }
  }

  void onSubmit(ChartDefinition chartDefinition) async {
    Success response;
    try {
      response = await this._socnetoService.createJobChartDefinition(job.id, chartDefinition);
    } on HttpException catch (e) {
      Toastr.error("Error", "Could not submit the chart definition: ${e.toString()}");
      return;
    }

    if (response.success) {
      Toastr.success("Success", "The chart was successfully created!");
    } else {
      Toastr.success("Error", "Could not create the chart :(");
    }

    this.createChartModal.close();
  }

}