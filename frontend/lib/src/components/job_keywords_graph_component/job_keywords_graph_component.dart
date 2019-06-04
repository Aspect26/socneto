import 'dart:js';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/content/deferred_content.dart';
import 'package:angular_components/material_button/material_button.dart';
import 'package:angular_components/material_checkbox/material_checkbox.dart';
import 'package:angular_components/material_input/material_auto_suggest_input.dart';
import 'package:angular_components/material_input/material_input.dart';
import 'package:angular_components/material_select/material_dropdown_select.dart';
import 'package:sw_project/src/components/posts_list/posts_list_component.dart';
import 'package:sw_project/src/models/Keyword.dart';
import 'package:sw_project/src/models/Post.dart';


@Component(
  selector: 'job-keywords-graph',
  directives: [
    DeferredContentDirective,

    MaterialAutoSuggestInputComponent,
    MaterialButtonComponent,
    MaterialCheckboxComponent,
    MaterialDropdownSelectComponent,
    MaterialChipsComponent,
    MaterialChipComponent,
    PostsListComponent,
    materialInputDirectives,
    NgFor,
    NgIf,
  ],
  providers: [
    materialProviders
  ],
  templateUrl: 'job_keywords_graph_component.html',
  styleUrls: ['job_keywords_graph_component.css'],
  encapsulation: ViewEncapsulation.None
)
class JobKeywordsGraphComponent implements AfterViewInit, AfterChanges {

  @Input() List<Post> posts = [];
  @ViewChild("keywordInput") MaterialAutoSuggestInputComponent keywordInput;

  List<Keyword> notSelectedKeywords = [];
  List<Keyword> selectedKeywords = [];
  String _currentKeywordInputText = "";

  List<Map> graphData = [];

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
    this.graphData = new List<Map>();

    var allKeywords = new List<Keyword>();
    var keywordsToPosts = new Map<String, List<Post>>();

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

      allKeywords.add(Keyword(keyword, keywordsToPosts[keyword].length));
      keywordsToPosts[keyword].sort((a, b) => a.postedAt.compareTo(b.postedAt));
      for (var post in keywordsToPosts[keyword]) {
        graphData.add({'y': post.sentiment, 'date': post.postedAt.toIso8601String()});
      }

      this.graphData.add({
        'keyword': keyword,
        'data': graphData,
      });
    }

    this.notSelectedKeywords = [];
    this.selectedKeywords = allKeywords;
  }

  void _refreshGraph() {

    var dataSets = [];
    var dataLabels = [];

    for (var selectedKeyword in this.selectedKeywords) {
      dataSets.add(this.graphData.firstWhere( (element) => element['keyword'] == selectedKeyword.keyword)['data']);
      dataLabels.add("${selectedKeyword.keyword}");
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
      this.notSelectedKeywords.remove(selected);
      this._refreshGraph();
    }
  }

  void onKeywordChipRemove(Keyword keyword) {
    if (this.selectedKeywords.contains(keyword)) {
      this.selectedKeywords.remove(keyword);
      this.notSelectedKeywords.add(keyword);
      this._notifyKeywordInputValuesChange();
      this._refreshGraph();
    }
  }

  ItemRenderer<dynamic> renderKeyword = (dynamic keywordChip) {
    return '${keywordChip.keyword} (${keywordChip.count})';
  };

  void _notifyKeywordInputValuesChange() {
    this.keywordInput.writeValue(_currentKeywordInputText + "a");
    this.keywordInput.writeValue(this._currentKeywordInputText.substring(0, this._currentKeywordInputText.length - 1));
  }

}
