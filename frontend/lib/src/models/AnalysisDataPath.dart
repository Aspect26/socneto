import 'package:sw_project/src/models/SocnetoAnalyser.dart';

class AnalysisDataPath {

  SocnetoAnalyser analyser;
  AnalysisProperty property;

  AnalysisDataPath(this.analyser, this.property);

  String toJsonPath() {
    return "${this.analyser}.${this.property.name}";
  }

}