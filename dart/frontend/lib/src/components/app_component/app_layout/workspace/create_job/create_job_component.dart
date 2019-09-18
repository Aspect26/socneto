import 'dart:html';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/create_job/component_select/components_select_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';


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
    MaterialSpinnerComponent,
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

  static final int _MAX_NUMBER_OF_POSTS = 1500;

  final SocnetoService _socnetoService;

  List<SocnetoComponent> availableSocialNetworks = [];
  List<SocnetoComponent> availableDataAnalyzers = [];

  bool loadingSocialNetworks = true;
  bool loadingDataAnalyzers = true;
  bool submitting = false;

  String jobName = "";
  String topic = "";
  List<SocnetoComponent> selectedSocialNetworks = [];
  List<SocnetoComponent> selectedDataAnalyzers = [];
  num maxPostsCount = _MAX_NUMBER_OF_POSTS;
  bool unlimitedPostsCount = false;

  CreateJobComponent(this._socnetoService);

  void ngOnInit() async {
    this._loadSocialNetworks();
    this._loadDataAnalyzers();
  }

  isQueryValid() {
    return this.jobName != null && this.jobName.isNotEmpty && this.topic != null
        && this.topic.isNotEmpty && this.selectedSocialNetworks.isNotEmpty && this.selectedDataAnalyzers.isNotEmpty;
  }

  onSubmit(UIEvent e) async {
    if (this.isQueryValid()) {
      try {
        this.submitting = true;
        await this._socnetoService.submitNewJob(this.topic, this.selectedSocialNetworks, this.selectedDataAnalyzers);
        this._clear();
        Toastr.success("New Job", "New job created successfully!");
      } on HttpException catch (e) {
        this._onSubmitError(e);
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

  _loadSocialNetworks() async {
    this.loadingSocialNetworks = true;
    this.availableSocialNetworks = [];

    try {
      this.availableSocialNetworks = await this._socnetoService.getAvailableNetworks();
    } catch (_) {
    } finally {
      this.loadingSocialNetworks = false;
    }
  }

  _loadDataAnalyzers() async {
    this.loadingDataAnalyzers = true;
    this.availableDataAnalyzers = [];

    try {
      this.availableDataAnalyzers = await this._socnetoService.getAvailableAnalyzers();
    } catch (_) {
    } finally {
      this.loadingDataAnalyzers = false;
    }
  }

  _onSubmitError(HttpException exception) {
    Toastr.error( "New Job", "Could not create the new job :(");
  }

  _clear() {
    this.jobName = "";
    this.topic = "";
    this.maxPostsCount = _MAX_NUMBER_OF_POSTS;
    this.unlimitedPostsCount = false;
    this.selectedSocialNetworks.clear();
    this.selectedDataAnalyzers.clear();
  }

}
