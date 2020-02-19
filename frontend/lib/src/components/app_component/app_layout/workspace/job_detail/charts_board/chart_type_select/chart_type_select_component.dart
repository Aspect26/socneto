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
class ChartTypeSelectComponent implements OnInit {

  final String _iconsLocation = "packages/sw_project/static/images/charts";
  final List<ChartType> chartTypes = [ChartType.Line, ChartType.Bar, ChartType.Pie, ChartType.Scatter];

  final _changeController = StreamController<ChartType>();
  @Output() Stream<ChartType> get change => _changeController.stream;

  ChartType selected;

  @override
  void ngOnInit() {
    this.onSelectionChange(chartTypes[0]);
  }

  void onSelectionChange(ChartType selectedType) {
    this.selected = selectedType;
    this._changeController.add(this.selected);
  }

  String getChartTitle(ChartType chartType) {
    switch (chartType) {
      case ChartType.Line: return "Line";
      case ChartType.Pie: return "Pie";
      case ChartType.Bar: return "Bar";
      case ChartType.Scatter: return "Scatter";
      case ChartType.PostsFrequency: return "Posts frequency";
    }
  }

  String getIcon(ChartType chartType) {
    String iconName = this._getIconName(chartType);
    return "${this._iconsLocation}/$iconName";
  }

  String _getIconName(ChartType chartType) {
    switch (chartType) {
      case ChartType.Line: return "line.png";
      case ChartType.Pie: return "pie.png";
      case ChartType.Bar: return "bar.png";
      case ChartType.Scatter: return "scatter.png";
      case ChartType.PostsFrequency: return "line.png";
    }
  }

}