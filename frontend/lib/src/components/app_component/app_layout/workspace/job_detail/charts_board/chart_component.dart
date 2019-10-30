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

  List<List<dynamic>> lineChartData = [];
  Map<String, num> pieChartData = {};

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
    switch (this.chartDefinition.chartType) {
      case ChartType.Line:
        this._transformDataPointsIntoLineChartData(chartDataPoints); break;
      case ChartType.Pie:
        this._transformDataPointsIntoPieChartData(chartDataPoints); break;
      case ChartType.Scatter:
        throw Exception("Scatter chart is not yet supported"); break;
    }
  }

  void _transformDataPointsIntoLineChartData(List<List<List<dynamic>>> chartDataPoints) {
    this.lineChartData = [];

    for (var currentLineData in chartDataPoints) {
      this.lineChartData.add([]);
      for (var dataPointValue in currentLineData) {
        // TODO: what if 'x' axis is not date?
        var datetimeString = dataPointValue[0] as String;
        var date = DateTime.parse(datetimeString.substring(0, 26));
        this.lineChartData.last.add({'x': date.toIso8601String(), 'y': dataPointValue[1]});
      }

      this.lineChartData.last.sort((a, b) => (a['x'] as String).compareTo(b['x']));
    }
  }

  void _transformDataPointsIntoPieChartData(List<List<List<dynamic>>> chartDataPoints) {
    this.pieChartData = {};

    var currentPieData = chartDataPoints[0];
    for (var dataPointValue in currentPieData) {
      var x = dataPointValue[0];
      var y = dataPointValue[1];

      this.pieChartData[x.toString()] = y;
    }
  }

  void _redrawChart() {
    var domSelector = "#${this.chartId}";

    switch (this.chartDefinition.chartType) {
      case ChartType.Line:
        List<String> labels = this.getLineChartLabels();
        SocnetoCharts.createLineChart(domSelector, this.lineChartData, labels); break;
      case ChartType.Pie:
        SocnetoCharts.createPieChart(domSelector, this.pieChartData); break;
      case ChartType.Scatter:
        throw Exception("Scatter chart is not yet supported");
    }

  }

  List<String> getLineChartLabels() {
    var jsonDataPathsExceptFirst = this.chartDefinition.jsonDataPaths.sublist(1);
    return jsonDataPathsExceptFirst.map<String>((jsonDataPath) => jsonDataPath.split("/").last).toList();
  }

}
