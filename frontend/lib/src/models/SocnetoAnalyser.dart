import 'package:sw_project/src/models/SocnetoComponent.dart';

import '../utils.dart';

class SocnetoAnalyser extends SocnetoComponent {

  final List<AnalysisProperty> properties;

  SocnetoAnalyser(String identifier, ComponentType type, this.properties) :
        super(identifier, type);

  SocnetoAnalyser.fromMap(Map data) :
        properties = (data["analysisProperties"] as Map<dynamic, dynamic>)?.entries?.map(
                (entry) => AnalysisProperty(entry.key, getEnumByString(AnalysisPropertyType.values, entry.value, AnalysisPropertyType.numberValue))
        )?.toList() ?? [],
        super.fromMap(data);
}


class AnalysisProperty {
  final String name;
  final AnalysisPropertyType type;

  AnalysisProperty(this.name, this.type);

  AnalysisProperty.fromMap(Map data) :
      name = data["identifier"],
      type = getEnumByString(AnalysisPropertyType.values, data["type"], AnalysisPropertyType.numberValue);
}


enum AnalysisPropertyType {
  numberValue,
  textValue,
  numberListValue,
  textListValue,
  numberMapValue,
  textMapValue,
}