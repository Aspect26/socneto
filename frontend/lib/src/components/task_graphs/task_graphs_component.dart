import 'dart:js';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/content/deferred_content.dart';
import 'package:angular_components/material_button/material_button.dart';
import 'package:angular_components/material_checkbox/material_checkbox.dart';
import 'package:angular_components/material_input/material_auto_suggest_input.dart';
import 'package:angular_components/material_input/material_input.dart';
import 'package:angular_components/material_select/material_dropdown_select.dart';
import 'package:angular_components/material_tab/material_tab.dart';
import 'package:angular_components/material_tab/material_tab_panel.dart';
import 'package:angular_components/model/selection/selection_model.dart';
import 'package:angular_components/laminate/popup/module.dart';


@Component(
  selector: 'task-graphs',
  directives: [
    DeferredContentDirective,
    MaterialTabPanelComponent,
    MaterialTabComponent,

    MaterialAutoSuggestInputComponent,
    MaterialButtonComponent,
    MaterialCheckboxComponent,
    MaterialDropdownSelectComponent,
    materialInputDirectives,
    NgFor,
    NgIf,
  ],
  providers: [
    materialProviders
  ],
  templateUrl: 'task_graphs_component.html',
  styleUrls: ['task_graphs_component.css'],
  encapsulation: ViewEncapsulation.None
)
class TaskGraphsComponent extends AfterViewInit {

  static var data = [
    {
      'keyword': 'Game of Thrones',
      'data': [{'y': 0.3, 'date': "2019-01-04"}, {'y': 0.8, 'date': "2019-02-04"}, {'y': 0.5, 'date': "2019-05-04"}],
    },
    {
      'keyword': 'Star Wars',
      'data': [{'y': 0.5, 'date': "2019-01-04"}, {'y': 0.2, 'date': "2019-04-04"}, {'y': 0.7, 'date': "2019-05-04"}],
    },
    {
      'keyword': 'Internet',
      'data': [{'y': 0.5, 'date': "2019-01-01"}, {'y': 0.2, 'date': "2019-02-04"}, {'y': 0.7, 'date': "2019-03-04"}, {'y': 0.1, 'date': "2019-04-04"}, {'y': 0.42, 'date': "2019-05-04"}, {'y': 0.3, 'date': "2019-06-04"}, {'y': 0.1, 'date': "2019-07-04"}]
    }
  ];

  List<String> myOptions = data.map( (element) => element['keyword'].toString()).toList();

  List<String> selectedKeywords = [];

  @override
  Future<Null> ngAfterViewInit() async {
    this.refreshGraph();
  }

  void refreshGraph() {
    var datasets = data.where( (element) => selectedKeywords.contains(element['keyword'])).toList().map( (element) => element['data']).toList();
    var dataLabels = data.where( (element) => selectedKeywords.contains(element['keyword'])).toList().map( (element) => element['keyword'].toString()).toList();

    context.callMethod('createLineChart', [".graph-line-chart", new JsObject.jsify(datasets), new JsObject.jsify(dataLabels)]);
  }

  void clear() {

  }

  void selectionChange(dynamic selected) {
    if (!this.selectedKeywords.contains(selected)) {
      this.selectedKeywords.add(selected);
    }

    this.refreshGraph();
  }

}