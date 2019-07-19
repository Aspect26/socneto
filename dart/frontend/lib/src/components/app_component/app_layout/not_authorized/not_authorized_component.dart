import 'package:angular/angular.dart';
import 'package:angular_router/angular_router.dart';

@Component(
    selector: 'not-authorized',
    templateUrl: 'not_authorized_component.html',
    styleUrls: [
      'not_authorized_component.css',
    ],
    directives: [
      routerDirectives,
    ],
  encapsulation: ViewEncapsulation.None,
)
class NotAuthorizedComponent {
}