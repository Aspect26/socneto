import 'dart:html';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:quiver/time.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/create_job/component_select/components_select_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/socneto_job_management_service.dart';


@Component(
  selector: 'create-job',
  directives: [
    formDirectives,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialIconComponent,
    MaterialCheckboxComponent,
    MaterialDropdownSelectComponent,
    MaterialSelectSearchboxComponent,
    DropdownSelectValueAccessor,
    MultiDropdownSelectValueAccessor,
    MaterialSelectDropdownItemComponent,
    MaterialDateTimePickerComponent,
    MaterialDateRangePickerComponent,
    MaterialTimePickerComponent,
    DateRangeInputComponent,
    materialInputDirectives,

    MaterialMultilineInputComponent,
    materialNumberInputDirectives,
    MaterialPaperTooltipComponent,
    MaterialTooltipTargetDirective,

    NgIf,

    ComponentsSelectComponent,
  ],
  templateUrl: 'create_job_component.html',
  styleUrls: ['create_job_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
    datepickerBindings
  ],
)
class CreateJobComponent implements OnInit {

  final SocnetoJobManagementService _socnetoJobManagementService;

  List<SocnetoComponent> availableSocialNetworks = [];
  List<SocnetoComponent> availableDataAnalyzers = [];

  bool loadingSocialNetworks = true;
  bool loadingDataAnalyzers = true;

  String jobName = "";
  String topic = "";
  List<SocnetoComponent> selectedSocialNetworks = [];
  List<SocnetoComponent> selectedDataAnalyzers = [];
  num maxPostsCount = 150;
  bool unlimitedPostsCount = false;
  bool isContinuous = false;
  DateTime dateFrom = DateTime.now();
  DatepickerComparison dateRange = DatepickerComparison.noComparison(DatepickerPreset.thisWeek(Clock()).range);
  DateTime timeFrom = DateTime.now();
  DateTime timeTo = DateTime.now();

  CreateJobComponent(this._socnetoJobManagementService);

  void ngOnInit() async {
    this._loadSocialNetworks();
    this._loadDataAnalyzers();
  }

  isQueryValid() {
    return this.jobName != null && this.jobName.isNotEmpty && this.topic != null
        && this.topic.isNotEmpty && this.selectedSocialNetworks.isNotEmpty && this.selectedDataAnalyzers.isNotEmpty;
  }

  onSubmit(UIEvent e) {
    if (this.isQueryValid()) {
      this._socnetoJobManagementService.submitNewJob(this.topic, this.selectedSocialNetworks, this.selectedDataAnalyzers).then((jobId) {
        this._clear(); Toastr.success("New Job", "New job created successfully!");
      }, onError: (error) {
        Toastr.error( "New Job", "Could not create the new job :(");
      });
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

  _loadSocialNetworks() async {
    this.loadingSocialNetworks = true;
    this.availableSocialNetworks = [];

    try {
      this.availableSocialNetworks = await this._socnetoJobManagementService.getAvailableNetworks();
    } catch (_) {
    } finally {
      this.loadingSocialNetworks = false;
    }
  }

  _loadDataAnalyzers() async {
    this.loadingDataAnalyzers = true;
    this.availableDataAnalyzers = [];

    try {
      this.availableDataAnalyzers = await this._socnetoJobManagementService.getAvailableAnalyzers();
    } catch (_) {
    } finally {
      this.loadingDataAnalyzers = false;
    }
  }

  _clear() {
    this.jobName = "";
    this.topic = "";
    this.maxPostsCount = 150;
    this.unlimitedPostsCount = false;
    this.isContinuous = false;
    this.dateFrom = DateTime.now();
    this.dateRange = DatepickerComparison.noComparison(DatepickerPreset.thisWeek(Clock()).range);
    this.timeFrom = DateTime.now();
    this.timeTo = DateTime.now();
    this.selectedSocialNetworks.clear();
    this.selectedDataAnalyzers.clear();
  }

}