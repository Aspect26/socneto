@JS()
library charts_interop;

import 'package:js/js.dart';
import 'package:js/js_util.dart' as js;

typedef LineChartFn = Function(String selector, List dataSets, List dataLabels);
typedef PieChartFn = Function(String selector, dynamic dataSet);

@JS()
class ChartsInterface {
  @JS() external LineChartFn get createLineChart;
  @JS() external PieChartFn get createPieChart;
}

@JS('Socneto.Charts')
external ChartsInterface get charts;

class SocnetoCharts {

  static void createLineChart(String selector, List<List<dynamic>> dataSets, List<String> dataLabels) {
    List jsData = [];
    for (var index = 0; index < dataSets.length; index++) {
      jsData.add(dataSets[index].map((d) => _mapToJsObject(d)).toList());
    }

    charts.createLineChart(selector, jsData, dataLabels);
  }

  static void createPieChart(String selector, Map<String, num> dataSet) {
    var jsData = _mapToJsObject(dataSet);
    charts.createPieChart(selector, jsData);
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
