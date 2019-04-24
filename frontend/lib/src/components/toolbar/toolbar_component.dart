import 'package:angular/angular.dart';
import 'package:angular_components/material_icon/material_icon.dart';

@Component(
    selector: 'toolbar',
    templateUrl: 'toolbar_component.html',
    styleUrls: const [
      'toolbar_component.css',
      'package:angular_components/app_layout/layout.scss.css'
    ],
    directives: [
      MaterialIconComponent
    ]
)
class ToolbarComponent {
  bool customWidth = false;
  bool end = false;
}