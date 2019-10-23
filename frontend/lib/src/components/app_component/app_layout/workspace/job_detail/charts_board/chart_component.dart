import 'dart:async';
import 'dart:js';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
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
    } on HttpException catch(e){
      Toastr.error("Analysis", "Could not fetch analyses for chart");
      print(e);
      return;
    }

    this._transformDataPointsIntoChartData(chartDataPoints);
    // TODO: The charts needs to be created after this element was already created
    Timer(Duration(milliseconds: 500), this._refreshGraph);
  }

  void _transformDataPointsIntoChartData(List<List<List<dynamic>>> chartDataPoints) {
    this.chartData = [];
    if (this.chartDefinition.chartType == ChartType.Line) {
      this._transformDataPointsIntoLineChartData(chartDataPoints);
    }
  }

  void _transformDataPointsIntoLineChartData(List<List<List<dynamic>>> chartDataPoints) {
    for (var currentLineData in chartDataPoints) {
      this.chartData.add([]);
      for (var dataPointValue in currentLineData) {
        // TODO: what if 'x' axis is not date?
        var datetimeString = dataPointValue[0] as String;
        var date = DateTime.parse(datetimeString.substring(0, 26));
        this.chartData.last.add({'date': date.toIso8601String(), 'value': dataPointValue[1]});
      }

      this.chartData.last.sort((a, b) => (a['date'] as String).compareTo(b['date']));
    }
  }

  void _refreshGraph() {
    var dataSets = this.chartData;
    
    var jsonDataPathsExceptFirst = List.from(this.chartDefinition.jsonDataPaths);
    jsonDataPathsExceptFirst.removeAt(0);
    var dataLabels = jsonDataPathsExceptFirst.map((jsonDataPath) => jsonDataPath.split("/").last);

    // TODO: make custom JS library from the graph-line-chart and interop it at least
    var domSelector = "#${this.chartId}";
    context.callMethod('createLineChart', [domSelector, JsObject.jsify(dataSets), JsObject.jsify(dataLabels)]);
  }

}