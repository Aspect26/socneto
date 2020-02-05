import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/models/PlatformStatus.dart';
import 'package:sw_project/src/services/platform_status_service.dart';


@Component(
  selector: 'platform-startup-info',
  directives: [
    formDirectives,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialIconComponent,
    MaterialCheckboxComponent,
    materialInputDirectives,

    MaterialMultilineInputComponent,
    materialNumberInputDirectives,
    MaterialPaperTooltipComponent,
    MaterialTooltipTargetDirective,

    NgIf,
    NgClass
  ],
  templateUrl: 'platform_startup_info_component.html',
  styleUrls: ['platform_startup_info_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class PlatformStartupInfoComponent implements OnInit {

  final _platformStartedController = StreamController<bool>();
  @Output() Stream<bool> get onPlatformStarted => _platformStartedController.stream;

  final PlatformStatusService _platformStatusService;

  bool isSubscribedToPlatformChanges = false;
  bool isBackendStopped = false;
  bool isStorageStopped = false;
  bool isJMSStopped = false;

  PlatformStartupInfoComponent(this._platformStatusService);

  @override
  void ngOnInit() {
    var platformStatus = this._platformStatusService.getCurrentStatus();
    this._updatePlatformStatus(null, platformStatus);
  }

  void _updatePlatformStatus(SocnetoComponentStatusChangedEvent changedEvent, PlatformStatus platformStatus) {
    this.isBackendStopped = platformStatus.backendStatus == SocnetoComponentStatus.STOPPED;
    this.isStorageStopped = platformStatus.storageStatus == SocnetoComponentStatus.STOPPED;
    this.isJMSStopped = platformStatus.jmsStatus == SocnetoComponentStatus.STOPPED;

    if (platformStatus.backendStatus == SocnetoComponentStatus.UNKNOWN ||
        platformStatus.storageStatus == SocnetoComponentStatus.UNKNOWN ||
        platformStatus.jmsStatus == SocnetoComponentStatus.UNKNOWN) {
      this._onUnknownStatusUpdate();
    } else if (this._isEverythingRunning()) {
      this._onEverythingRunningUpdate();
    } else {
      this._onSomethingNotRunningUpdate();
    }
  }

  bool _isEverythingRunning() => !this.isBackendStopped && !this.isStorageStopped && !this.isJMSStopped;

  void _onEverythingRunningUpdate() {
    this._unsubscribeFromPlatformChanges();
    this._platformStartedController.add(true);
  }

  void _onSomethingNotRunningUpdate() {
    this._subscribeToPlatformChanges();
  }

  void _onUnknownStatusUpdate() {
    this._subscribeToPlatformChanges();
  }

  void _subscribeToPlatformChanges() {
    if (!this.isSubscribedToPlatformChanges) {
      this._platformStatusService.subscribeToChanges(this, this._updatePlatformStatus);
      this.isSubscribedToPlatformChanges = true;
    }
  }

  void _unsubscribeFromPlatformChanges() {
    if (this.isSubscribedToPlatformChanges) {
      this._platformStatusService.unsubscribeFromChanges(this);
      this.isSubscribedToPlatformChanges = false;
    }
  }

}
