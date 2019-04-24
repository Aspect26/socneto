import 'package:angular/angular.dart';
import 'package:sw_project/src/components/task_detail/task_detail_component.dart';
import 'package:sw_project/src/components/tasks_list/tasks_list_component.dart';
import 'package:sw_project/src/components/toolbar/toolbar_component.dart';
import 'package:sw_project/src/models/Task.dart';

// AngularDart info: https://webdev.dartlang.org/angular
// Components info: https://webdev.dartlang.org/components

@Component(
  selector: 'my-app',
  styleUrls: ['app_component.css'],
  templateUrl: 'app_component.html',
  directives: [ToolbarComponent, TasksListComponent, TaskDetailComponent],
)
class AppComponent {

  @ViewChild(TaskDetailComponent) TaskDetailComponent taskDetailComponent;

  taskSelected(Task task) {
    taskDetailComponent.setTask(task);
  }

}
