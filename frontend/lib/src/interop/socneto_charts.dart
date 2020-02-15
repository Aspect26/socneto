@JS()
library charts_interop;

import 'package:js/js.dart';
import 'package:js/js_util.dart' as js;

typedef LineChartFn = Function(String selector, List dataSets, List dataLabels, bool isXDateTime, String xAxisLabel);
typedef PieChartFn = Function(String selector, dynamic dataSet);
typedef ScatterChartFn = Function(String selector, dynamic dataSet);

@JS()
class ChartsInterface {
  @JS() external LineChartFn get createLineChart;
  @JS() external PieChartFn get createPieChart;
  @JS() external ScatterChartFn get createScatterChart;
}

@JS('Socneto.Charts')
external ChartsInterface get charts;

class SocnetoCharts {

  static void createLineChart(String selector, List<List<dynamic>> dataSets, List<String> dataLabels, bool isXDateTime, String xAxisLabel) {
    List jsData = [];
    for (var index = 0; index < dataSets.length; index++) {
      jsData.add(dataSets[index].map((d) => _mapToJsObject(d)).toList());
    }

    charts.createLineChart(selector, jsData, dataLabels, isXDateTime, xAxisLabel);
  }

  static void createPieChart(String selector, Map<String, num> dataSet) {
    var jsData = _mapToJsObject(dataSet);
    charts.createPieChart(selector, jsData);
  }

  static void createScatterChart(String selector, List<Map<String, num>> dataSet) {
    List jsData = [];
    for (var index = 0; index < dataSet.length; index++) {
      jsData.add(_mapToJsObject(dataSet[index]));
    }

    charts.createScatterChart(selector, jsData);
  }

  static Object _mapToJsObject(Map<dynamic,dynamic> a){
    var object = js.newObject();
    a.forEach((k, v) {
      var key = k;
      var value = v;
      js.setProperty(object, key, value);
    });
    return object;
  }

}
