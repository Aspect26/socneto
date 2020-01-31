import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/utils/angular/scroll_host/angular_2.dart';

@Component(
  selector: 'quick-guide',
  templateUrl: 'quick_guide_component.html',
  styleUrls: ['quick_guide_component.css'],
  directives: [
    MaterialStepperComponent,
    StepDirective,
    SummaryDirective,
    MaterialButtonComponent,
    materialInputDirectives,

    NgFor,
  ],
  providers: [materialProviders, scrollHostProviders, overlayBindings],
)
class QuickGuideComponent {
}