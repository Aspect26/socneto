import 'dart:html';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'login',
  templateUrl: 'login_component.html',
  styleUrls: [
    'package:angular_components/css/mdc_web/card/mdc-card.scss.css',
    'login_component.css'
  ],
  directives: [
    routerDirectives,
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
  exports: [RoutePaths, Routes],
  providers: [
    ClassProvider(SocnetoService)
  ],
  encapsulation: ViewEncapsulation.None
)
class LoginComponent {

  final SocnetoService _socnetoService;
  final Router _router;

  String username;
  String password;

  LoginComponent(this._socnetoService, this._router);

  bool isInputValid() {
    return username != null && username.isNotEmpty && password != null && password.isNotEmpty;
  }

  onLogin(UIEvent event) {
    if (this.isInputValid()) {
      this._socnetoService.login(username, password).then((userId) {
        // TODO: this userID should be somwhere as a constant
        this._router.navigate(RoutePaths.workspace.toUrl(parameters: {"userId": '${userId}'}));
      }, onError: (error) {
        this._onCantLogin();
      });
    }
  }
  
  _onCantLogin() {
    // TODO: more info
    Toastr.error("Login", "Can't login with the provided credentials");
  }

}