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
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_list/create_job/component_attributes_component.dart';
import 'package:sw_project/src/components/shared/component_select/components_select_component.dart';


@Component(
  selector: 'verification-modal',
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
    ComponentAttributesComponent,
    MaterialStepperComponent,
    StepDirective,
    SummaryDirective,
    MaterialToggleComponent,
    materialInputDirectives,
    NgFor,
    NgIf,
  ],
  templateUrl: 'verification_modal.html',
  styleUrls: ['verification_modal.css'],
  providers: [materialProviders, scrollHostProviders, overlayBindings],
)
class VerificationModal {

  @Input() String title;

  @Input() Function confirmedCallback;

  final _submitController = StreamController<bool>();
  @Output() Stream<bool> get submit => _submitController.stream;

  bool displayed = false;
  bool submitting = false;

  void show() {
    this.reset();
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
      await this.confirmedCallback();
      this._submitController.add(true);
    } finally {
      this.submitting = false;
      this.close();
    }
  }

  void onCanceled() {
    this._submitController.add(false);
    this.close();
  }

}
