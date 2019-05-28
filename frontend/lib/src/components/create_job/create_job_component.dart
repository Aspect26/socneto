import 'dart:html';

import 'package:angular/angular.dart';
import 'package:angular_components/focus/focus.dart';
import 'package:angular_components/material_button/material_button.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_input/material_input.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/services/socneto_service.dart';

@Component(
  selector: 'create-job',
  directives: [
    formDirectives,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialIconComponent,
    materialInputDirectives,
    NgIf
  ],
  templateUrl: 'create_job_component.html',
  styleUrls: ['create_job_component.css'],
  providers: [
    ClassProvider(SocnetoService)
  ],
)
class CreateJobComponent {

  String query = "";
  SocnetoService _socnetoService;

  CreateJobComponent(this._socnetoService);

  isQueryValid() {
    return query != null && query.isNotEmpty;
  }



  onSubmit(UIEvent e) {
    if (this.isQueryValid()) {
      this._socnetoService.submitNewJob(this.query)
          .catchError((a) => {print("error: $a")} )
          .then((jobId) => { print("result: $jobId")});
    }
  }

}