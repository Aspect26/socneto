import '../utils.dart';

class ChartDefinition {
  final String dataJsonPath;
  final ChartType chartType;

  ChartDefinition(this.dataJsonPath, this.chartType);

  ChartDefinition.fromMap(Map data) :
      dataJsonPath = data["dataJsonPath"],
      chartType = getEnumByString(ChartType.values, data["chartType"], ChartType.Line);
}

enum ChartType {
  Line,
  Pie
}