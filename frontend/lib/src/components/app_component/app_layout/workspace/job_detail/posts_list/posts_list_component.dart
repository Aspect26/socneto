import 'dart:convert';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
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
import 'package:sw_project/src/components/shared/paginator/Paginator.dart';
import 'package:sw_project/src/components/shared/paginator/paginator_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';

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
    MaterialProgressComponent,
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
    PaginatorComponent,
    NgFor,
    NgIf,
  ],
  providers: [
    overlayBindings
  ],
  templateUrl: 'posts_list_component.html',
  styleUrls: ['posts_list_component.css'],
  encapsulation: ViewEncapsulation.None
)
class PostsListComponent implements AfterChanges {

    static const int PAGE_SIZE = 20;
    static const JsonEncoder JSON_ENCODER = JsonEncoder.withIndent("  ");

    @Input() Job job;
    List<AnalyzedPost> posts = [];
    Paginator paginator = Paginator(0, 1, PAGE_SIZE);
    bool loading = false;

    final SocnetoService _socnetoService;

    PostsListComponent(this._socnetoService);

    @override
    void ngAfterChanges() async {
        await this._updateDisplayedPosts();
    }

    void onPageChange(int page) async {
        this.paginator.currentPage = page;
        await this._updateDisplayedPosts();
    }

    String prettyJson(dynamic obj) => JSON_ENCODER.convert(obj);

    void _updateDisplayedPosts() async {
        this.loading = true;
        try {
            var paginatedPosts = await this._socnetoService.getJobPosts(job.id, paginator.currentPage, paginator.pageSize);
            this.posts = paginatedPosts.posts;
            this.paginator = Paginator(paginatedPosts.paging.totalSize, paginatedPosts.paging.page, paginatedPosts.paging.pageSize);
        } on HttpException catch (e) {
            Toastr.error("Posts", "Unable to load posts on given page");
            print(e);
            this.posts = [];
            this.paginator = Paginator(0, 1, PAGE_SIZE);
        } finally {
            this.loading = false;
        }
    }
}