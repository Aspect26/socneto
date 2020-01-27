import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/interop/socneto_charts.dart';

abstract class ChartStrategy {

  setData(ChartDefinition chartDefinition, List<List<List<dynamic>>> dataSet);

  redrawChart(String domSelector);

}

class LineChartStrategy implements ChartStrategy {

  List<List<dynamic>> _chartData = [];
  ChartDefinition _chartDefinition;

  @override
  setData(ChartDefinition chartDefinition, List<List<List<dynamic>>> dataSet) {
    this._chartData = [];
    this._chartDefinition = chartDefinition;

    for (var currentLineData in dataSet) {
      this._chartData.add([]);
      for (var dataPointValue in currentLineData) {
        var xValue = this._chartDefinition.isXDateTime ? DateTime.parse(dataPointValue[0] as String).toIso8601String() : dataPointValue[0];
        this._chartData.last.add({'x': xValue, 'y': dataPointValue[1]});
      }

      this._chartData.last.sort((a, b) => (a['x']).compareTo(b['x']));
    }
  }

  @override
  redrawChart(String domSelector) {
    List<String> labels = this._getLineChartLabels();
    SocnetoCharts.createLineChart(domSelector, this._chartData, labels, this._chartDefinition.isXDateTime);
  }

  List<String> _getLineChartLabels() {
    return this._chartDefinition.analysisDataPaths.map<String>((dataPath) => dataPath.property).toList();
  }

}

class PieChartStrategy implements ChartStrategy {

  Map<String, num> _chartData = {};

  @override
  setData(ChartDefinition chartDefinition, List<List<List<dynamic>>> dataSet) {
    this._chartData = {};

    var currentPieData = dataSet[0];
    for (var dataPointValue in currentPieData) {
      var x = dataPointValue[0];
      var y = dataPointValue[1];

      this._chartData[x.toString()] = y;
    }
  }

  @override
  redrawChart(String domSelector) {
    SocnetoCharts.createPieChart(domSelector, this._chartData);
  }

}

class ScatterChartStrategy implements ChartStrategy {

  List<Map<String, num>> _chartData = [];

  @override
  setData(ChartDefinition chartDefinition, List<List<List<dynamic>>> dataSet) {
    this._chartData = [];

    var currentPieData = dataSet[0];
    for (var dataPointValue in currentPieData) {
      var x = dataPointValue[0];
      var y = dataPointValue[1];
      this._chartData.add({ "x": x, "y": y });
    }
  }

  @override
  redrawChart(String domSelector) {
    SocnetoCharts.createScatterChart(domSelector, this._chartData);
  }

}