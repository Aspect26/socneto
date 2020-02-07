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
import 'package:sw_project/src/components/shared/component_select/components_select_component.dart';
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

  final List<String> _TWITTER_KEYS = ["api_key", "api_secret_key", "access_token", "access_token_secret"];
  final List<String> _REDDIT_KEYS = ["app_id", "app_secret", "refresh_token"];

  @Input() List<SocnetoComponent> acquirers = [];
  @Input() bool disabled = false;

  final _credentialsChangeController = StreamController<List<AcquirerWithCredentials>>();
  @Output() Stream<List<AcquirerWithCredentials>> get onCredentialsChange => _credentialsChangeController.stream;

  List<AcquirerWithCredentials> acquirersWithCredentials = [];
  AcquirerWithCredentials selectedAcquirer;
  bool useCustomTwitterCredentials;

  @override
  void ngAfterChanges() {
    this.acquirersWithCredentials = [];
    this.acquirers.forEach((acquirer) {
      this.acquirersWithCredentials.add(AcquirerWithCredentials(acquirer, [MutableTuple("", "")]));
    });

    this.selectedAcquirer = this.acquirersWithCredentials.isNotEmpty? this.acquirersWithCredentials[0] : null;
  }

  bool hasAllTwitterKeys(List<MutableTuple> credentials) =>
      this._hasAllKeys(credentials, this._TWITTER_KEYS);

  bool hasAllRedditKeys(List<MutableTuple> credentials) =>
      this._hasAllKeys(credentials, this._REDDIT_KEYS);

  void onRemove(List<MutableTuple> credentials, MutableTuple credential) =>
      credentials.remove(credential);

  void onAddNew(List<MutableTuple> credentials) =>
      this._addNewEmpty(credentials);
  
  void onAddDefaultTwitter(List<MutableTuple> credentials) =>
      this._addNew(credentials, this._TWITTER_KEYS);

  void onAddDefaultReddit(List<MutableTuple> credentials) =>
      this._addNew(credentials, this._REDDIT_KEYS);

  void _addNewEmpty(List<MutableTuple> credentials) =>
      credentials.add(MutableTuple("", ""));

  bool _hasAllKeys(List<MutableTuple> credentials, List<String> keys) {
    for (var key in keys) {
      if (!this._credentialsHasField(credentials, key)) {
        return false;
      }
    }

    return true;
  }

  void _addNew(List<MutableTuple> credentials, List<String> keys) =>
      keys.forEach((key) => this._addIfAbsent(credentials, key));
  
  void _addIfAbsent(List<MutableTuple> credentials, String key) {
    if (!this._credentialsHasField(credentials, key)) {
      credentials.add(MutableTuple<String, String>(key, ""));
    }
  }

  bool _credentialsHasField(List<MutableTuple> credentials, String key) {
    var credentialWithKey = credentials.firstWhere((credential) => credential.item1 == key, orElse: () => null);
    return credentialWithKey != null;
  }
  
}

class MutableTuple<T1, T2> {
  T1 item1;
  T2 item2;

  MutableTuple(this.item1, this.item2);
}

class AcquirerWithCredentials {
  SocnetoComponent acquirer;
  List<MutableTuple<String, String>> credentials;

  AcquirerWithCredentials(
      this.acquirer,
      this.credentials
  );
}