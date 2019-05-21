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
import 'package:sw_project/src/models/Post.dart';


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
class TaskGraphsComponent implements AfterViewInit, AfterChanges {

  @Input() List<Post> posts = [];
  @ViewChild("keywordInput") MaterialAutoSuggestInputComponent keywordInput;

  List<String> selectKeywordOptions = [];
  List<String> selectedKeywords = [];
  List<Map> data = [];

  String _currentKeywordInputText = "";

  @override
  void ngAfterViewInit() {
    this._transformPostsIntoData();
    this._refreshGraph();
  }

  @override
  void ngAfterChanges() {
    if (this.posts.isEmpty) {
      return;
    }

    this._transformPostsIntoData();
    this._refreshGraph();
  }

  void _transformPostsIntoData() {
    this.data = new List<Map>();
    Map<String, List<Post>> keywordsToPosts = new Map();

    for (var post in this.posts) {
      for (var keyword in post.keywords) {
        if (keywordsToPosts.containsKey(keyword)) {
          keywordsToPosts[keyword].add(post);
        } else {
          keywordsToPosts[keyword] = new List<Post>();
          keywordsToPosts[keyword].add(post);
        }
      };
    }

    for (var keyword in keywordsToPosts.keys) {
      List<Map> graphData = [];

      keywordsToPosts[keyword].sort((a, b) => a.postedAt.compareTo(b.postedAt));
      for (var post in keywordsToPosts[keyword]) {
        graphData.add({'y': post.sentiment, 'date': post.postedAt.toIso8601String()});
      }

      this.data.add({
        'keyword': keyword,
        'data': graphData,
      });
    }

    this.selectKeywordOptions = [];
    this.selectedKeywords = keywordsToPosts.keys.toList();
  }

  void _refreshGraph() {
    var dataSets = [];
    var dataLabels = [];

    for (var selectedKeyword in this.selectedKeywords) {
      dataSets.add(this.data.firstWhere( (element) => element['keyword'] == selectedKeyword)['data']);
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
      this._refreshGraph();
    }
  }

  void onKeywordChipRemove(String keyword) {
    if (this.selectedKeywords.contains(keyword)) {
      this.selectedKeywords.remove(keyword);
      this.selectKeywordOptions.add(keyword);
      this._notifyKeywordInputValuesChange();
      this._refreshGraph();
    }
  }

  void _notifyKeywordInputValuesChange() {
    this.keywordInput.writeValue(_currentKeywordInputText + "a");
    this.keywordInput.writeValue(this._currentKeywordInputText.substring(0, this._currentKeywordInputText.length - 1));
  }

}