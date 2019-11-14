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
        // TODO: what if 'x' axis is not date?
        var datetimeString = dataPointValue[0] as String;
        var date = DateTime.parse(datetimeString.substring(0, 26));
        this._chartData.last.add({'x': date.toIso8601String(), 'y': dataPointValue[1]});
      }

      this._chartData.last.sort((a, b) => (a['x'] as String).compareTo(b['x']));
    }
  }

  @override
  redrawChart(String domSelector) {
    List<String> labels = this._getLineChartLabels();
    SocnetoCharts.createLineChart(domSelector, this._chartData, labels);
  }

  List<String> _getLineChartLabels() {
    var jsonDataPathsExceptFirst = this._chartDefinition.jsonDataPaths.sublist(1);
    return jsonDataPathsExceptFirst.map<String>((jsonDataPath) => jsonDataPath.split("/").last).toList();
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
  @override
  setData(ChartDefinition chartDefinition, List<List<List>> dataSet) {
    // TODO: implement setData
    return null;
  }

  @override
  redrawChart(String domSelector) {
    SocnetoCharts.createScatterChart(domSelector);
  }

}