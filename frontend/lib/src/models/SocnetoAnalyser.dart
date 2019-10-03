import 'package:sw_project/src/models/SocnetoComponent.dart';

class SocnetoAnalyser extends SocnetoComponent {

  final List<String> properties;

  SocnetoAnalyser(String identifier, ComponentType type, ComponentSpecialization specialization, this.properties) :
        super(identifier, type, specialization);

  SocnetoAnalyser.fromMap(Map data) :
        properties = ["polarity", "accuracy"], // TODO: mock here!
        super.fromMap(data);

}