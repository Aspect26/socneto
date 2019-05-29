import 'package:angular/angular.dart';
import 'package:angular_components/app_layout/material_persistent_drawer.dart';
import 'package:angular_components/app_layout/material_temporary_drawer.dart';
import 'package:angular_components/content/deferred_content.dart';
import 'package:angular_components/material_button/material_button.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_toggle/material_toggle.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/components/create_job/create_job_component.dart';
import 'package:sw_project/src/components/job_detail/job_detail_component.dart';
import 'package:sw_project/src/components/jobs_list/jobs_list_component.dart';
import 'package:sw_project/src/routes.dart';

@Component(
    selector: 'app-layout',
    templateUrl: 'app_layout_component.html',
    styleUrls: const [
      'app_layout_component.css',
      'package:angular_components/app_layout/layout.scss.css'
    ],
    directives: [
      routerDirectives,

      JobsListComponent,
      JobDetailComponent,
      CreateJobComponent,

      DeferredContentDirective,

      MaterialIconComponent,
      MaterialButtonComponent,
      MaterialIconComponent,
      MaterialPersistentDrawerDirective,
      MaterialTemporaryDrawerComponent,
      MaterialToggleComponent,
      MaterialListItemComponent
    ],
  encapsulation: ViewEncapsulation.None,
  exports: [RoutePaths, Routes],
)
class AppLayoutComponent {
}