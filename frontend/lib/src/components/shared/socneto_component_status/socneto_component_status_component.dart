import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/models/PlatformStatus.dart';


@Component(
  selector: 'socneto-component-status',
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
  templateUrl: 'socneto_component_status_component.html',
  styleUrls: ['socneto_component_status_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class SocnetoComponentStatusComponent {

  @Input() SocnetoComponentStatus componentStatus;

  SocnetoComponentStatus get unknownStatus => SocnetoComponentStatus.UNKNOWN;
  SocnetoComponentStatus get stoppedStatus => SocnetoComponentStatus.STOPPED;
  SocnetoComponentStatus get startingStatus => SocnetoComponentStatus.STARTING;
  SocnetoComponentStatus get runningStatus => SocnetoComponentStatus.RUNNING;

}
