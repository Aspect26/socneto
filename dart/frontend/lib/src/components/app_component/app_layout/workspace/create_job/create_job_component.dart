import 'dart:html';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:quiver/time.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/SocialNetwork.dart';
import 'package:sw_project/src/services/socneto_data_service.dart';


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

    NgIf
  ],
  templateUrl: 'create_job_component.html',
  styleUrls: ['create_job_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
    datepickerBindings
  ],
)
class CreateJobComponent {

  static const List<SocialNetwork> _socialNetworks = [
    SocialNetwork("Facebook", "www.facebook.com"),
    SocialNetwork("Twitter", "www.twitter.com"),
    SocialNetwork("Reddit", "www.reddit.com"),
  ];

  final SocnetoDataService _socnetoDataService;

  String jobName = "";
  num maxPostsCount = 150;
  bool unlimitedPostsCount = false;
  bool isContinuous = false;
  DateTime dateFrom = new DateTime.now();
  DatepickerComparison dateRange = DatepickerComparison.noComparison(DatepickerPreset.thisWeek(new Clock()).range);
  DateTime timeFrom = DateTime.now();
  DateTime timeTo = DateTime.now();

  final SelectionModel<SocialNetwork> socialNetworksSelectionModel = SelectionModel<SocialNetwork>.multi();
  final StringSelectionOptions<SocialNetwork> socialNetworksOptions = StringSelectionOptions<SocialNetwork>(_socialNetworks);

  CreateJobComponent(this._socnetoDataService);

  static ItemRenderer socialNetworkItemRenderer = newCachingItemRenderer<dynamic>(
          (network) => "${network.name} (${network.url})");

  isQueryValid() {
    return this.jobName != null && this.jobName.isNotEmpty && this.socialNetworksSelectionModel.isNotEmpty;
  }

  onSubmit(UIEvent e) {
    if (this.isQueryValid()) {
      this._socnetoDataService.submitNewJob(this.jobName).then((jobId) {
        this._clear(); Toastr.success("New Job", "New job created successfully!");
      }, onError: (error) {
        Toastr.error( "New Job", "Could not create the new job :(");
      });
    }
  }

  String get socialSelectLabel {
    var selectedSocialNetworks = this.socialNetworksSelectionModel.selectedValues;
    if (selectedSocialNetworks.isEmpty) {
      return "No social networks selected";
    } else {
      return selectedSocialNetworks.map((network) => network.name).join(", ");
    }
  }

  _clear() {
    this.jobName = "";
    this.maxPostsCount = 150;
    this.unlimitedPostsCount = false;
    this.isContinuous = false;
    this.dateFrom = DateTime.now();
    this.dateRange = DatepickerComparison.noComparison(DatepickerPreset.thisWeek(new Clock()).range);
    this.timeFrom = DateTime.now();
    this.timeTo = DateTime.now();
    this.socialNetworksSelectionModel.clear();
  }

}