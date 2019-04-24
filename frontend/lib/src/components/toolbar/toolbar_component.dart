import 'package:angular/angular.dart';

@Component(
    selector: 'toolbar',
    templateUrl: 'toolbar_component.html',
    styleUrls: const ['toolbar_component.css'],
    directives: []
)
class ToolbarComponent {
  bool customWidth = false;
  bool end = false;
}