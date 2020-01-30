import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/app_layout/material_persistent_drawer.dart';
import 'package:angular_components/app_layout/material_temporary_drawer.dart';
import 'package:angular_components/content/deferred_content.dart';
import 'package:angular_components/material_button/material_button.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_toggle/material_toggle.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/workspace_component.dart';
import 'package:sw_project/src/components/shared/socneto_component_status/socneto_component_status_component.dart';
import 'package:sw_project/src/models/PlatformStatus.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';

@Component(
    selector: 'app-layout',
    templateUrl: 'app_layout_component.html',
    styleUrls: [
      'app_layout_component.css',
      'package:angular_components/app_layout/layout.scss.css'
    ],
    directives: [
      routerDirectives,
      WorkspaceComponent,
      SocnetoComponentStatusComponent,

      DeferredContentDirective,

      MaterialIconComponent,
      MaterialButtonComponent,
      MaterialIconComponent,
      MaterialPersistentDrawerDirective,
      MaterialTemporaryDrawerComponent,
      MaterialToggleComponent,
      MaterialListItemComponent,

      NgIf
    ],
  encapsulation: ViewEncapsulation.None,
  exports: [RoutePaths, Routes],
)
class AppLayoutComponent {

  final SocnetoService _socnetoService;
  final Duration repeatInterval = const Duration(seconds:5);

  Timer currentPollingTimer;
  PlatformStatus currentPlatformStatus;
  SocnetoComponentStatus backendStatus;

  AppLayoutComponent(this._socnetoService);

  void onDrawerToggled(bool visible) {
    if (visible) {
      if (this.currentPollingTimer != null) {
        this.currentPollingTimer.cancel();
      }

      this.onPollStatus();
      this.currentPollingTimer = Timer.periodic(repeatInterval, (Timer t) => this.onPollStatus());
    } else {
      if (this.currentPollingTimer != null) {
        this.currentPollingTimer.cancel();
        this.currentPollingTimer = null;
      }
    }
  }
  
  void onPollStatus() async {
    try {
      this.currentPlatformStatus = await this._socnetoService.getPlatformStatus();
      this.backendStatus = SocnetoComponentStatus.RUNNING;
    } catch (HttpException) {
      this.currentPlatformStatus = null;
      this.backendStatus = SocnetoComponentStatus.STOPPED;
    }
  }

}