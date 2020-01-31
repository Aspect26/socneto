import 'package:angular/angular.dart';
import 'package:sw_project/src/components/app_component/app_layout/app_layout_component.dart';
import 'package:sw_project/src/services/platform_status_service.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'socneto-app',
  styleUrls: ['app_component.css'],
  templateUrl: 'app_component.html',
  directives: [AppLayoutComponent],
  providers: [
    ClassProvider(SocnetoService),
    ClassProvider(PlatformStatusService)
  ],
  encapsulation: ViewEncapsulation.None
)
class AppComponent {

  final SocnetoService _socnetoService;

  AppComponent(this._socnetoService) {
    this._socnetoService.tryLoginFromLocalStorage();
  }

}
