import '../utils.dart';
import 'AnalysisDataPath.dart';

class ChartDefinition {
  final List<AnalysisDataPath> analysisDataPaths;
  final ChartType chartType;
  final bool isXDateTime;

  ChartDefinition(this.analysisDataPaths, this.chartType, this.isXDateTime);

  ChartDefinition.fromMap(Map data) :
        analysisDataPaths = (data["analysis_data_paths"] as List<dynamic>).map((x) => AnalysisDataPath.fromMap(x)).toList(),
        chartType = getEnumByString(ChartType.values, data["chart_type"], ChartType.Line),
        isXDateTime = data["is_x_datetime"];
}

enum ChartType {
  Line,
  Pie,
  Scatter,
}
