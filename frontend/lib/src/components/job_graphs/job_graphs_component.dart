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


@Component(
  selector: 'job-graphs',
  directives: [
    DeferredContentDirective,
    MaterialTabPanelComponent,
    MaterialTabComponent,
    MaterialAutoSuggestInputComponent,
    MaterialButtonComponent,
    MaterialCheckboxComponent,
    MaterialDropdownSelectComponent,
    MaterialChipsComponent,
    MaterialChipComponent,
    materialInputDirectives,
    NgFor,
    NgIf,
  ],
  providers: [
    materialProviders
  ],
  templateUrl: 'job_graphs_component.html',
  styleUrls: ['job_graphs_component.css'],
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

  @ViewChild("keywordInput") MaterialAutoSuggestInputComponent keywordInput;
  List<String> selectKeywordOptions = data.map( (element) => element['keyword'].toString()).toList();
  List<String> selectedKeywords = [];

  String _currentKeywordInputText = "";

  @override
  Future<Null> ngAfterViewInit() async {
    this.refreshGraph();
  }

  void refreshGraph() {
    var dataSets = [];
    var dataLabels = [];

    for (var selectedKeyword in this.selectedKeywords) {
      dataSets.add(data.firstWhere( (element) => element['keyword'] == selectedKeyword)['data']);
      dataLabels.add(selectedKeyword);
    }

    context.callMethod('createLineChart', [".graph-line-chart", new JsObject.jsify(dataSets), new JsObject.jsify(dataLabels)]);
  }

  void addKeywordTextChange(String text) {
    this._currentKeywordInputText = text != null? text : "";
  }

  void addKeywordSelectionChange(dynamic selected) {
    if (selected != null && selected != "" && !this.selectedKeywords.contains(selected)) {
      this.selectedKeywords.add(selected);
      this.keywordInput.writeValue("");
      this.selectKeywordOptions.remove(selected);
      this.refreshGraph();
    }
  }

  void onKeywordChipRemove(String keyword) {
    if (this.selectedKeywords.contains(keyword)) {
      this.selectedKeywords.remove(keyword);
      this.selectKeywordOptions.add(keyword);
      this._notifyKeywordInputValuesChange();
      this.refreshGraph();
    }
  }

  void _notifyKeywordInputValuesChange() {
    this.keywordInput.writeValue(_currentKeywordInputText + "a");
    this.keywordInput.writeValue(this._currentKeywordInputText.substring(0, this._currentKeywordInputText.length - 1));
  }

}