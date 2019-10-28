@JS()
library charts_interop;

import 'package:js/js.dart';

typedef ChartsFn = Function(String selector, List dataSets, List dataLabels);

@JS()
class ChartsInterface {
  @JS() external ChartsFn get createLineChart;
  @JS() external ChartsFn get createPieChart;
}

@JS('Socneto.Charts')
external ChartsInterface get charts;

class SocnetoCharts {

  static void createLineChart(String selector, List dataSets, List dataLabels) => charts.createLineChart(selector, dataSets, dataLabels);
  static void createPieChart(String selector, List dataSets, List dataLabels) => charts.createPieChart(selector, dataSets, dataLabels);

}
