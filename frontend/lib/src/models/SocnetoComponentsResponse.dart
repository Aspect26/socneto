import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';

class SocnetoComponentsResponse {
  final List<SocnetoComponent> components;

  const SocnetoComponentsResponse(this.components);

  SocnetoComponentsResponse.fromMap(Map data) :
        components = (data["components"] as List).map((componentData) => SocnetoComponent.fromMap(componentData)).toList();
}


class SocnetoAnalysersResponse extends SocnetoComponentsResponse {
  const SocnetoAnalysersResponse(List<SocnetoAnalyser> components) : super(components);

  SocnetoAnalysersResponse.fromMap(Map data) :
        super((data["components"] as List).map((componentData) => SocnetoAnalyser.fromMap(componentData)).toList());

}