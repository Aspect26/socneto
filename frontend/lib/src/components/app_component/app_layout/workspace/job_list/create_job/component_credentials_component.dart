import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_components/utils/angular/scroll_host/angular_2.dart';
import 'package:sw_project/src/components/shared/component_select/components_select_component.dart';
import 'package:sw_project/src/models/Credentials.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';


@Component(
  selector: 'component-credentials',
  directives: [
    DeferredContentDirective,
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    MaterialDropdownSelectComponent,
    MaterialRadioComponent,
    MaterialRadioGroupComponent,
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
    ModalComponent,
    ComponentsSelectComponent,
    MaterialStepperComponent,
    StepDirective,
    SummaryDirective,
    MaterialToggleComponent,
    materialInputDirectives,
    NgFor,
    NgIf,
  ],
  templateUrl: 'component_credentials_component.html',
  styleUrls: ['component_credentials_component.css'],
  providers: [materialProviders, scrollHostProviders, overlayBindings],
)
class ComponentCredentialsComponent implements AfterChanges {

  @Input() List<SocnetoComponent> acquirers = [];
  @Input() bool disabled = false;

  List<AcquirerWithCredentials> acquirersWithCredentials = [];
  AcquirerWithCredentials selectedAcquirer;
  bool useCustomTwitterCredentials;

  @override
  void ngAfterChanges() {
    this.acquirersWithCredentials = [];
    this.acquirers.forEach((acquirer) {
      this.acquirersWithCredentials.add(AcquirerWithCredentials(acquirer, false, TwitterCredentials(), false, RedditCredentials()));
    });

    this.selectedAcquirer = this.acquirersWithCredentials.isNotEmpty? this.acquirersWithCredentials[0] : null;
  }

}

class AcquirerWithCredentials {
  SocnetoComponent acquirer;

  bool useCustomTwitterCredentials = false;
  TwitterCredentials twitterCredentials;

  bool useCustomRedditCredentials = false;
  RedditCredentials redditCredentials;

  AcquirerWithCredentials(
      this.acquirer,
      this.useCustomTwitterCredentials,
      this.twitterCredentials,
      this.useCustomRedditCredentials,
      this.redditCredentials
  );
}