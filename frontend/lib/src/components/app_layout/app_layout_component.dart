import 'package:angular/angular.dart';
import 'package:angular_components/app_layout/material_persistent_drawer.dart';
import 'package:angular_components/app_layout/material_temporary_drawer.dart';
import 'package:angular_components/content/deferred_content.dart';
import 'package:angular_components/material_button/material_button.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_toggle/material_toggle.dart';
import 'package:sw_project/src/components/task_detail/task_detail_component.dart';
import 'package:sw_project/src/components/tasks_list/tasks_list_component.dart';
import 'package:sw_project/src/models/Task.dart';

@Component(
    selector: 'app-layout',
    templateUrl: 'app_layout_component.html',
    styleUrls: const [
      'app_layout_component.css',
      'package:angular_components/app_layout/layout.scss.css'
    ],
    directives: [
      TasksListComponent,
      TaskDetailComponent,

      DeferredContentDirective,

      MaterialIconComponent,
      MaterialButtonComponent,
      MaterialIconComponent,
      MaterialPersistentDrawerDirective,
      MaterialTemporaryDrawerComponent,
      MaterialToggleComponent,
      MaterialListItemComponent
    ],
    encapsulation: ViewEncapsulation.None
)
class AppLayoutComponent {
  bool customWidth = false;
  bool end = false;

  @ViewChild(TaskDetailComponent) TaskDetailComponent taskDetailComponent;

  taskSelected(Task task) {
    taskDetailComponent.setTask(task);
  }
}