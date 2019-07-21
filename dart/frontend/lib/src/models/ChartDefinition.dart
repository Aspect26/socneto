class ChartDefinition {
  final String dataJsonPath;

  ChartDefinition(this.dataJsonPath);

  ChartDefinition.fromMap(Map data) :
      dataJsonPath = data["dataJsonPath"];
}