import '../utils.dart';

class ChartDefinition {
  final List<String> jsonDataPaths;
  final ChartType chartType;

  ChartDefinition(this.jsonDataPaths, this.chartType);

  ChartDefinition.fromMap(Map data) :
      jsonDataPaths = (data["jsonDataPaths"] as List<dynamic>).cast<String>(),
      chartType = getEnumByString(ChartType.values, data["chartType"], ChartType.Line);
}

enum ChartType {
  Line,
  Pie,
  Scatter,
  Bar
}