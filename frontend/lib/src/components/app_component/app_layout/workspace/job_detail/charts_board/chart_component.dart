import 'dart:async';
import 'package:js/js_util.dart' as js;

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/interop/socneto_charts.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'chart',
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
  templateUrl: 'chart_component.html',
  styleUrls: ['chart_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class ChartComponent implements AfterChanges {

  final SocnetoService _socnetoService;

  @Input() ChartDefinition chartDefinition;
  @Input() String jobId;
  @Input() String chartId;

  List<List<dynamic>> chartData = [];

  ChartComponent(this._socnetoService);

  @override
  void ngAfterChanges() async {
    this._refreshChart();
  }

  void _refreshChart() async {
    var chartDataPoints;

    try {
      chartDataPoints = await this._socnetoService.getChartData(this.jobId, this.chartDefinition);
    } on HttpException {
      Toastr.error("Analysis", "Could not fetch analyses for chart");
      return;
    }

    this._transformDataPointsIntoChartData(chartDataPoints);
    // TODO: The charts needs to be created after this element was already created
    Timer(Duration(milliseconds: 500), this._redrawChart);
  }

  void _transformDataPointsIntoChartData(List<List<List<dynamic>>> chartDataPoints) {
    this.chartData = [];
    switch (this.chartDefinition.chartType) {
      case ChartType.Line:
        this._transformDataPointsIntoLineChartData(chartDataPoints); break;
      case ChartType.Pie:
        this._transformDataPointsIntoLineChartData(chartDataPoints); break;
      case ChartType.Scatter:
        throw Exception("Scatter chart is not yet supported"); break;
    }
  }

  void _transformDataPointsIntoLineChartData(List<List<List<dynamic>>> chartDataPoints) {
    for (var currentLineData in chartDataPoints) {
      this.chartData.add([]);
      for (var dataPointValue in currentLineData) {
        // TODO: what if 'x' axis is not date?
        var datetimeString = dataPointValue[0] as String;
        var date = DateTime.parse(datetimeString.substring(0, 26));
        this.chartData.last.add({'x': date.toIso8601String(), 'y': dataPointValue[1]});
      }

      this.chartData.last.sort((a, b) => (a['x'] as String).compareTo(b['x']));
    }

    this._transformChartDataToJsObjects();
  }

  void _transformChartDataToJsObjects() {
    for (var index = 0; index < this.chartData.length; index++) {
      this.chartData[index] = this.chartData[index].map((d) => _mapToJsObject(d)).toList();
    }
  }

  void _redrawChart() {
    var dataSets = this.chartData;
    
    var jsonDataPathsExceptFirst = List.from(this.chartDefinition.jsonDataPaths);
    jsonDataPathsExceptFirst.removeAt(0);
    var dataLabels = jsonDataPathsExceptFirst.map((jsonDataPath) => jsonDataPath.split("/").last).toList();

    var domSelector = "#${this.chartId}";

    switch (this.chartDefinition.chartType) {
      case ChartType.Line:
        SocnetoCharts.createLineChart(domSelector, dataSets, dataLabels); break;
      case ChartType.Pie:
        SocnetoCharts.createPieChart(domSelector, dataSets, dataLabels); break;
      case ChartType.Scatter:
        throw Exception("Scatter chart is not yet supported");
    }

  }

  Object _mapToJsObject(Map<dynamic,dynamic> a){
    var object = js.newObject();
    a.forEach((k, v) {
      var key = k;
      var value = v;
      js.setProperty(object, key, value);
    });
    return object;
  }

}