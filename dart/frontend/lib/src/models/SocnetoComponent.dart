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
  final String title;
  final ComponentType type;
  final ComponentSpecialization specialization;

  const SocnetoComponent(this.identifier, this.title, this.type, this.specialization);

  SocnetoComponent.fromMap(Map data) :
        identifier = data["id"] ?? "",
        title = data["title"] ?? "Unnamed",
        type = getEnumByString(ComponentType.values, data["type"], ComponentType.unknown),
        specialization = getEnumByString(ComponentSpecialization.values, data["specialization"], ComponentSpecialization.other);
}