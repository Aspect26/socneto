import 'package:angular/angular.dart';
import 'package:angular_components/content/deferred_content.dart';
import 'package:angular_components/material_tab/material_tab.dart';
import 'package:angular_components/material_tab/material_tab_panel.dart';

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
class TaskGraphsComponent {

}