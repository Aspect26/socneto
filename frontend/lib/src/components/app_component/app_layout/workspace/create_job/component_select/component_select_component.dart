import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';


@Component(
  selector: 'component-select',
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

    NgIf
  ],
  templateUrl: 'component_select_component.html',
  styleUrls: ['component_select_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class ComponentSelectComponent implements AfterChanges {

  final String _iconsLocation = "packages/sw_project/static/images/components";

  @Input() SocnetoComponent component;

  final _changeController = StreamController<bool>();
  @Output() Stream<bool> get change => _changeController.stream;

  String iconPath;
  bool selected = true;

  ComponentSelectComponent();

  void ngAfterChanges() {
    this._setIcon();
  }

  void toggled() {
    this.selected = !this.selected;
    this._changeController.add(this.selected);
  }

  void _setIcon() {
    String iconName = "component_default.png";
    switch (this.component.specialization) {
      case ComponentSpecialization.facebook: iconName = "facebook.png"; break;
      case ComponentSpecialization.twitter: iconName = "twitter.png"; break;
      case ComponentSpecialization.reddit: iconName = "reddit.png"; break;
      case ComponentSpecialization.other:
        if (this.component.type == ComponentType.dataAcquirer) {
          iconName = "data_acquirer_default.png";
        } else if (this.component.type == ComponentType.dataAnalyzer) {
          iconName = "data_analyzer_default.png";
        } else {
          iconName = "component_default.png";
        }
        break;
    }

    this.iconPath = "${this._iconsLocation}/${iconName}";
  }

}