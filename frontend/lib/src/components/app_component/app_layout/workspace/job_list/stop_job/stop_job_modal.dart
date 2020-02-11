import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_components/utils/angular/scroll_host/angular_2.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_list/create_job/component_credentials_component.dart';
import 'package:sw_project/src/components/shared/component_select/components_select_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/JmsJobResponse.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'stop-job-modal',
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
    MaterialTabComponent,
    MaterialSpinnerComponent,
    MaterialYesNoButtonsComponent,
    ModalComponent,
    ComponentsSelectComponent,
    ComponentCredentialsComponent,
    MaterialStepperComponent,
    StepDirective,
    SummaryDirective,
    MaterialToggleComponent,
    materialInputDirectives,
    NgFor,
    NgIf,
  ],
  templateUrl: 'stop_job_modal.html',
  styleUrls: ['stop_job_modal.css'],
  providers: [materialProviders, scrollHostProviders, overlayBindings],
)
class StopJobModal {

  final _submitController = StreamController<JobStatus>();
  @Output() Stream<JobStatus> get submit => _submitController.stream;

  final SocnetoService _socnetoService;

  Job job;
  bool displayed = false;
  bool submitting = false;

  StopJobModal(this._socnetoService);

  void show(Job job) {
    this.reset();
    this.job = job;
    this.displayed = true;
  }

  void close() {
    this.displayed = false;
  }

  void reset() {
    this.submitting = false;
  }

  bool isShown() {
    return this.displayed;
  }

  void onConfirmed() async {
    try {
      this.submitting = true;
      final jobStatus = await this._socnetoService.stopJob(job.id);
      this._submitController.add(jobStatus);
    } on HttpException catch (e) {
      Toastr.httpError(e);
      print(e);
    } finally {
      this.submitting = false;
    }
  }

}
