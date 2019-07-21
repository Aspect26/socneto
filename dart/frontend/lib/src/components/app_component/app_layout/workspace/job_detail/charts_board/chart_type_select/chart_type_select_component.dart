import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';


@Component(
  selector: 'chart-type-select',
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
    NgFor
  ],
  templateUrl: 'chart_type_select_component.html',
  styleUrls: ['chart_type_select_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class ChartTypeSelectComponent {

  final String _iconsLocation = "packages/sw_project/static/images/charts";
  final List<ChartType> chartTypes = [ChartType.line, ChartType.pie];

  final _changeController = StreamController<ChartType>();
  @Output() Stream<ChartType> get change => _changeController.stream;

  ChartType selected;

  ChartTypeSelectComponent();


  void onSelectionChange(ChartType selectedType) {
    this.selected = selectedType;
    this._changeController.add(this.selected);
  }

  String getChartTitle(ChartType chartType) {
    switch (chartType) {
      case ChartType.line: return "Line";
      case ChartType.pie: return "Pie";
      default: return "Chart";
    }
  }

  String getIcon(ChartType chartType) {
    String iconName = "";
    switch (chartType) {
      case ChartType.line: iconName = "line.png"; break;
      case ChartType.pie: iconName = "pie.png"; break;
    }

    return "${this._iconsLocation}/$iconName";
  }

}