class AnalysisDataPath {

  String analyserId;
  String property;

  AnalysisDataPath(this.analyserId, this.property);

  AnalysisDataPath.fromMap(Map data) :
      analyserId = data["analyser_id"],
      property = data["property"];

}