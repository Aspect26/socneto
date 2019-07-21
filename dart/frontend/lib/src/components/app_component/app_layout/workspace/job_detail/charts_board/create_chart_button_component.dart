import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/Job.dart';
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
    try {
      await this._socnetoService.createJobChartDefinition(job.id, jsonPath);
    } catch (e) {
      this._handleHttpException(e);
      return;
    }
    this.showCreateDialog = false;
  }

  void _reset() {
    this.jsonPath = "";
    this.errorMessage = null;
  }

  // TODO: catch HttpException here when corresponding branch is merged
  void _handleHttpException(e) {
    Toastr.error("Error", "Could not submit the job");
  }

}