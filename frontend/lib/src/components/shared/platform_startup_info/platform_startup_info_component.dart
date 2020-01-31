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
  bool isBackendRunning = false;
  bool isStorageRunning = false;
  bool isJMSRunning = false;

  PlatformStartupInfoComponent(this._platformStatusService);

  @override
  void ngOnInit() {
    var platformStatus = this._platformStatusService.getCurrentStatus();
    this._updatePlatformStatus(null, platformStatus);
  }

  void _updatePlatformStatus(SocnetoComponentStatusChangedEvent changedEvent, PlatformStatus platformStatus) {
    this.isBackendRunning = platformStatus.backendStatus == SocnetoComponentStatus.RUNNING;
    this.isStorageRunning = platformStatus.storageStatus == SocnetoComponentStatus.RUNNING;
    this.isJMSRunning = platformStatus.jmsStatus == SocnetoComponentStatus.RUNNING;

    if (this.isBackendRunning && this.isStorageRunning && this.isJMSRunning && this.isSubscribedToPlatformChanges) {
      this._platformStatusService.unsubscribeFromChanges(this);
      this.isSubscribedToPlatformChanges = false;
      this._platformStartedController.add(true);
    }

    if ((!this.isBackendRunning || !this.isStorageRunning || !this.isJMSRunning) && !this.isSubscribedToPlatformChanges) {
      this._platformStatusService.subscribeToChanges(this, this._updatePlatformStatus);
      this.isSubscribedToPlatformChanges = true;
    }
  }

}
