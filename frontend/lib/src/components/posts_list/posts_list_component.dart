import 'package:angular/angular.dart';
import 'package:angular_components/focus/focus.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/laminate/components/modal/modal.dart';
import 'package:angular_components/laminate/overlay/module.dart';
import 'package:angular_components/material_button/material_button.dart';
import 'package:angular_components/material_dialog/material_dialog.dart';
import 'package:angular_components/material_expansionpanel/material_expansionpanel.dart';
import 'package:angular_components/material_expansionpanel/material_expansionpanel_auto_dismiss.dart';
import 'package:angular_components/material_expansionpanel/material_expansionpanel_set.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_input/material_input.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_components/material_yes_no_buttons/material_yes_no_buttons.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/services/task_service.dart';

@Component(
  selector: 'posts-list',
  directives: [
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialExpansionPanel,
    MaterialExpansionPanelAutoDismiss,
    MaterialExpansionPanelSet,
    MaterialDialogComponent,
    MaterialInputComponent,
    materialInputDirectives,
    MaterialYesNoButtonsComponent,
    ModalComponent,
    NgModel,

    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    NgFor,
    NgIf,
  ],
  providers: [
    ClassProvider(TaskService),
    overlayBindings
  ],
  templateUrl: 'posts_list_component.html',
  styleUrls: ['posts_list_component.css'],
  encapsulation: ViewEncapsulation.None
)
class PostsListComponent {

  @Input() List<Post> posts = [];
  var errorMessage = "";

}