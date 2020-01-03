import 'package:sw_project/src/models/SocnetoAnalyser.dart';

class AnalysisDataPath {

  String analyserId;
  AnalysisProperty property;

  AnalysisDataPath(this.analyserId, this.property);

  AnalysisDataPath.fromMap(Map data) :
      analyserId = data["analyser_id"],
      property = AnalysisProperty.fromMap(data["property"]);

}