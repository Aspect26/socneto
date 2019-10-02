import 'package:sw_project/src/utils.dart';

enum ComponentType {
  dataAcquirer,
  dataAnalyzer,
  unknown
}

enum ComponentSpecialization {
  facebook,
  twitter,
  reddit,
  other,
}

class SocnetoComponent {
  final String identifier;
  final ComponentType type;
  final ComponentSpecialization specialization;

  const SocnetoComponent(this.identifier, this.type, this.specialization);

  SocnetoComponent.fromMap(Map data) :
        identifier = data["componentId"] ?? "",
        type = getEnumByString(ComponentType.values, data["componentType"], ComponentType.unknown),
        specialization = getEnumByString(ComponentSpecialization.values, data["specialization"], ComponentSpecialization.other);
}
