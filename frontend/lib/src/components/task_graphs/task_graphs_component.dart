import 'package:angular/angular.dart';
import 'package:angular_components/content/deferred_content.dart';
import 'package:angular_components/material_tab/material_tab.dart';
import 'package:angular_components/material_tab/material_tab_panel.dart';

import 'dart:js';


@Component(
  selector: 'task-graphs',
  directives: [
    DeferredContentDirective,
    MaterialTabPanelComponent,
    MaterialTabComponent,
  ],
  templateUrl: 'task_graphs_component.html',
  styleUrls: ['task_graphs_component.css'],
  encapsulation: ViewEncapsulation.None
)
class TaskGraphsComponent extends AfterViewInit {

  @override
  Future<Null> ngAfterViewInit() async {


    var width = 800;
    var height = 200;

    var dataset = [{'y': 0.3, 'date': "2019-01-04"}, {'y': 0.8, 'date': "2019-02-04"}, {'y': 0.5, 'date': "2019-05-04"}];
    var dataset2 = [{'y': 0.5, 'date': "2019-01-04"}, {'y': 0.2, 'date': "2019-04-04"}, {'y': 0.7, 'date': "2019-05-04"}];
    var dataset3 = [{'y': 0.5, 'date': "2019-01-01"}, {'y': 0.2, 'date': "2019-02-04"}, {'y': 0.7, 'date': "2019-03-04"},
      {'y': 0.1, 'date': "2019-04-04"}, {'y': 0.42, 'date': "2019-05-04"}, {'y': 0.3, 'date': "2019-06-04"}, {'y': 0.1, 'date': "2019-07-04"}, ];

    var datasets = [dataset, dataset2, dataset3];
    var dataLabels = ["Game of Thrones", "Star Wars", "Internet"];

    context.callMethod('createLineChart', [".graph-line-chart", new JsObject.jsify(datasets), new JsObject.jsify(dataLabels), width, height]);

  }

}