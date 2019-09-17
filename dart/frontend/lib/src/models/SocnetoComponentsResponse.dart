import 'package:sw_project/src/models/SocnetoComponent.dart';

class SocnetoComponentsResponse {
  final List<SocnetoComponent> components;

  const SocnetoComponentsResponse(this.components);

  SocnetoComponentsResponse.fromMap(Map data) :
        components = (data["components"] as List).map((componentData) => SocnetoComponent.fromMap(componentData)).toList();
}
