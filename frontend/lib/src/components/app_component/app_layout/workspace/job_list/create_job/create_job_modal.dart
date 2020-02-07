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
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'create-job-modal',
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
  templateUrl: 'create_job_modal.html',
  styleUrls: ['create_job_modal.css'],
  providers: [materialProviders, scrollHostProviders, overlayBindings],
)
class CreateJobModal {

  final _submitController = StreamController<JobStatus>();
  @Output() Stream<JobStatus> get submit => _submitController.stream;

  final SocnetoService _socnetoService;

  bool displayed = false;
  String errorMessage;

  List<SocnetoComponent> availableSocialNetworks = [];
  List<SocnetoComponent> availableDataAnalyzers = [];
  static final ItemRenderer languagesItemRenderer = newCachingItemRenderer<dynamic>((language) => language == "cs"? "Český" : language == "en"? "English" : "UNKNOWN");

  bool loadingSocialNetworks = true;
  bool loadingDataAnalyzers = true;
  bool submitting = false;

  String jobName = "";
  String topic = "";
  SingleSelectionModel languageSelection = SingleSelectionModel();
  List<SocnetoComponent> selectedSocialNetworks = [];
  List<SocnetoComponent> selectedDataAnalyzers = [];
  List<AcquirerWithCredentials> acquirersWithCredentials = [];

  CreateJobModal(this._socnetoService);

  void show() {
    this.reset();
    this.displayed = true;
  }

  void close() {
    this.displayed = false;
  }

  void reset() {
    this.jobName = "";
    this.topic = "";
    this.languageSelection = SingleSelectionModel();
    this.languageSelection.select("en");
    this.selectedSocialNetworks.clear();
    this.selectedDataAnalyzers.clear();
    this.errorMessage = null;

    this._loadSocialNetworks();
    this._loadDataAnalyzers();
  }

  bool isShown() {
    return this.displayed;
  }

  void onCloseDialog() {
    this.displayed = false;
  }

  void onSubmit() async {
    if (this.isJobDefinitionCorrect()) {
      try {
        this.submitting = true;
        final credentials = this._getAllCredentials();
        final jobStatus = await this._socnetoService.submitNewJob(this.jobName, this.topic, this.selectedSocialNetworks,
            this.selectedDataAnalyzers, this.languageSelection.selectedValue, credentials);
        this.reset();
        this._submitController.add(jobStatus);
        Toastr.success("New Job", "Job successfully submited");
      } on HttpException {
        Toastr.error( "New Job", "Could not create the new job :(");
      } finally {
        this.submitting = false;
      }
    }
  }

  onSocialNetworksSelectionChange(List<SocnetoComponent> networks) {
    this.selectedSocialNetworks = networks;
  }

  onSocialNetworksRefresh() {
    this._loadSocialNetworks();
  }

  onDataAnalyzersSelectionChange(List<SocnetoComponent> analyzers) {
    this.selectedDataAnalyzers = analyzers;
  }

  onDataAnalyzersRefresh() {
    this._loadDataAnalyzers();
  }

  onCredentialsChange(List<AcquirerWithCredentials> credentials) {
    this.acquirersWithCredentials = credentials;
  }

  bool isJobDefinitionCorrect() {
    return this.jobName.isNotEmpty && this.topic.isNotEmpty && this.selectedDataAnalyzers.isNotEmpty && this.selectedSocialNetworks.isNotEmpty;
  }

  _loadSocialNetworks() async {
    this.loadingSocialNetworks = true;
    this.availableSocialNetworks = [];

    try {
      this.availableSocialNetworks = await this._socnetoService.getAvailableAcquirers();
    } catch (e) {
      Toastr.error("Acquirers", "Error fetching acquirers");
      print(e);
    } finally {
      this.loadingSocialNetworks = false;
    }
  }

  _loadDataAnalyzers() async {
    this.loadingDataAnalyzers = true;
    this.availableDataAnalyzers = [];

    try {
      this.availableDataAnalyzers = await this._socnetoService.getAvailableAnalyzers();
    } catch (e) {
      Toastr.error("Acquirers", "Error fetching analysers");
      print(e);
    } finally {
      this.loadingDataAnalyzers = false;
    }
  }

  Map<String, Map<String, String>> _getAllCredentials() {
    Map<String, Map<String, String>> acquirersToCredentials = {};

    this.acquirersWithCredentials.forEach((credentials) {
      Map<String, String> acquirerCredentialsMap = this._createCredentialsMap(credentials.credentials);
      String acquirerIdentifier = credentials.acquirer.identifier;

      acquirersToCredentials[acquirerIdentifier] = acquirerCredentialsMap;
    });

    return acquirersToCredentials;
  }

  Map<String, String> _createCredentialsMap(List<MutableTuple<String, String>> data) {
    Map<String, String> resultMap = {};
    data.forEach((credentialTuple) {
      resultMap[credentialTuple.item1] = credentialTuple.item2;
    });

    return resultMap;
  }

}
