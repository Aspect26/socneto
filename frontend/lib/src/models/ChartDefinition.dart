import '../utils.dart';
import 'AnalysisDataPath.dart';

class ChartDefinition {
  final String id;
  final String title;
  final List<AnalysisDataPath> analysisDataPaths;
  final ChartType chartType;
  final bool isXDateTime;

  ChartDefinition(this.title, this.analysisDataPaths, this.chartType, this.isXDateTime, { this.id = "" });

  ChartDefinition.fromMap(Map data) :
        id = data["id"],
        title = data["title"],
        analysisDataPaths = (data["analysis_data_paths"] as List<dynamic>).map((x) => AnalysisDataPath.fromMap(x)).toList(),
        chartType = getEnumByString(ChartType.values, data["chart_type"], ChartType.Line),
        isXDateTime = data["is_x_datetime"];
}

enum ChartType {
  Line,
  Pie,
  Bar,
  Scatter,

  PostsFrequency,
  LanguageFrequency,
  AuthorFrequency
}
