import 'package:sw_project/src/utils.dart';

enum ComponentType {
  DataAcquirer,
  DataAnalyser,
  Unknown
}


class SocnetoComponent {
  final String identifier;
  final ComponentType type;

  const SocnetoComponent(this.identifier, this.type);

  SocnetoComponent.fromMap(Map data) :
        identifier = data["identifier"] ?? "",
        type = getEnumByString(ComponentType.values, data["componentType"], ComponentType.Unknown);
}
