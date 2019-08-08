import 'dart:async';
import 'dart:js';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';


@Component(
  selector: 'chart',
  directives: [
    formDirectives,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialIconComponent,
    MaterialCheckboxComponent,
    materialInputDirectives,

    MaterialMultilineInputComponent,
    materialNumberInputDirectives,
    MaterialPaperTooltipComponent,
    MaterialTooltipTargetDirective,

    NgIf,
    NgFor
  ],
  templateUrl: 'chart_component.html',
  styleUrls: ['chart_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class ChartComponent implements AfterChanges {

  // TODO: currently supports only line chart

  @Input() ChartDefinition chartDefinition;
  @Input() List<AnalyzedPost> analyzedPosts;
  @Input() String chartId;

  List<dynamic> graphData = [];

  @override
  void ngAfterChanges() {
    if (this.chartDefinition != null && this.analyzedPosts != null && this.chartId != null) {
      // The charts needs to be created after this element was already created
      Timer(Duration(milliseconds: 500), this._showChart);
    }
  }

  void _showChart() {
    if (this.analyzedPosts.isEmpty) {
      return;
    }

    this._transformPostsIntoData();
    this._refreshGraph();
  }

  void _transformPostsIntoData() {
    this.graphData = List<dynamic>();
    this.analyzedPosts.sort((a, b) {
      if (a.post.postedAt == null) Toastr.error("Post data", "Post '${a.post.text}' is missing date");
      if (b.post.postedAt == null) Toastr.error("Post data", "Post '${b.post.text}' is missing date");
      return a.post.postedAt.compareTo(b.post.postedAt);
    });

    for (var post in this.analyzedPosts) {
      var value = this._getAnalysisValue(post.analyses);
      this.graphData.add({'y': value, 'date': post.post.postedAt.toIso8601String()});
    }
  }

  void _refreshGraph() {
    var dataSets = [this.graphData];
    var dataLabels = [this.chartDefinition.dataJsonPath.split(".")[1]];

    // TODO: make custom JS library from the graph-line-chart and interop it at least
    var domSelector = "#${this.chartId}";
    context.callMethod('createLineChart', [domSelector, JsObject.jsify(dataSets), JsObject.jsify(dataLabels)]);
  }

  dynamic _getAnalysisValue(dynamic analyses) {
    var pathParts = this.chartDefinition.dataJsonPath.split(".");
    if (pathParts.length != 2) {
      Toastr.error("Error", "Wrong data path: ${this.chartDefinition.dataJsonPath}");
      return 0;
    }

    for (dynamic analysis in analyses) {
      if (analysis[pathParts[0]] != null && analysis[pathParts[0]][pathParts[1]] != null) {
        return analysis[pathParts[0]][pathParts[1]]["value"];
      }
    }

    return 0;
  }

}