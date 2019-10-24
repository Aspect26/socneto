import 'package:sw_project/src/models/SocnetoComponent.dart';

import '../utils.dart';

class SocnetoAnalyser extends SocnetoComponent {

  final List<AnalysisProperty> properties;

  SocnetoAnalyser(String identifier, ComponentType type, this.properties) :
        super(identifier, type);

  SocnetoAnalyser.fromMap(Map data) :
        properties = (data["analysisProperties"] as List<dynamic>)?.map((propertyData) => AnalysisProperty.fromMap(propertyData))?.toList(),
        super.fromMap(data);

}


class AnalysisProperty {
  final String name;
  final AnalysisPropertyType type;

  AnalysisProperty.fromMap(Map data) :
      name = data["name"],
      type = getEnumByString(AnalysisPropertyType.values, data["type"], AnalysisPropertyType.Number);
}


enum AnalysisPropertyType {
  Number,
  String,
  NumberList,
  StringList
}