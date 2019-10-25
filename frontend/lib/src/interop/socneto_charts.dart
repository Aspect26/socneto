@JS()
library charts_interop;

import 'package:js/js.dart';

typedef ChartsFn = Function(String selector, List dataSets, List dataLabels);

class ChartsInterface {
  external ChartsFn get createLineChart;
}

@JS('Socneto.Charts')
external ChartsInterface get asd;

class SocnetoCharts {

  static void createLineChart(String title, List dataSets, List dataLabels) => asd.createLineChart(title, dataSets, dataLabels);

}
