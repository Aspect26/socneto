import 'package:sw_project/src/utils.dart';

enum ComponentType {
  DATA_ACQUIRER,
  DATA_ANALYSER,
  UNKNOWN
}


class SocnetoComponent {
  final String identifier;
  final ComponentType type;

  const SocnetoComponent(this.identifier, this.type);

  SocnetoComponent.fromMap(Map data) :
        identifier = data["identifier"] ?? "",
        type = getEnumByString(ComponentType.values, data["component_type"], ComponentType.UNKNOWN);
}
