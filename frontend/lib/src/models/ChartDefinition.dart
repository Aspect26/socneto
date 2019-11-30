import '../utils.dart';
import 'AnalysisDataPath.dart';

class ChartDefinition {
  final List<AnalysisDataPath> analysisDataPaths;
  final ChartType chartType;

  ChartDefinition(this.analysisDataPaths, this.chartType);

  ChartDefinition.fromMap(Map data) :
        analysisDataPaths = (data["analysisDataPaths"] as List<dynamic>).cast<AnalysisDataPath>(),
        chartType = getEnumByString(ChartType.values, data["chartType"], ChartType.Line);
}

enum ChartType {
  Line,
  Pie,
  Scatter,
}