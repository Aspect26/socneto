import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/create_job/component_select/component_select_component.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';


@Component(
  selector: 'components-select',
  directives: [
    formDirectives,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialIconComponent,
    MaterialCheckboxComponent,
    MaterialProgressComponent,
    materialInputDirectives,

    MaterialMultilineInputComponent,
    materialNumberInputDirectives,
    MaterialPaperTooltipComponent,
    MaterialTooltipTargetDirective,

    ComponentSelectComponent,

    NgIf,
    NgFor
  ],
  templateUrl: 'components_select_component.html',
  styleUrls: ['components_select_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class ComponentsSelectComponent implements AfterChanges {

  @Input() List<SocnetoComponent> components;
  @Input() bool loading = false;

  final _changeController = StreamController<List<SocnetoComponent>>();
  @Output() Stream<List<SocnetoComponent>> get change => _changeController.stream;

  final _refreshController = StreamController<bool>();
  @Output() Stream<bool> get refresh => _refreshController.stream;

  List<SocnetoComponent> selectedComponents = [];

  ComponentsSelectComponent();

  void ngAfterChanges() {
    this.selectedComponents = []..addAll(this.components);
  }

  void componentToggled(SocnetoComponent component, bool checked) {
    if (checked) {
      this.selectedComponents.add(component);
    } else {
      this.selectedComponents.remove(component);
    }

    this._changeController.add(this.selectedComponents);
  }

  void onRefreshClick() {
    this._refreshController.add(true);
  }

}
