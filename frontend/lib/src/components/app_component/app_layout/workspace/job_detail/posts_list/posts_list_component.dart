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
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
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
    MaterialChipComponent,
    MaterialDialogComponent,
    MaterialDateRangePickerComponent,
    MaterialInputComponent,
    MaterialPopupComponent,
    MaterialProgressComponent,
    materialInputDirectives,
    PopupSourceDirective,
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

    // TODO: the filter related things should be extracted into new FilterPostsComponent
    static const int PAGE_SIZE = 20;
    static const JsonEncoder JSON_ENCODER = JsonEncoder.withIndent("  ");

    @Input() Job job;

    List<Post> posts = [];
    Paginator paginator = Paginator(0, 1, PAGE_SIZE);
    bool loading = false;

    bool showFilterPopup = false;
    bool useDateFilter = false;
    bool useContainsWordsFilter = false;
    bool useExcludeWordsFilter = false;

    DateRange dateRange;

    List<String> containedWords = [];
    String newContainedWord = "";

    List<String> excludedWords = [];
    String newExcludedWord = "";

    final SocnetoService _socnetoService;

    PostsListComponent(this._socnetoService);

    @override
    void ngAfterChanges() async {
        await this._updateDisplayedPosts();
    }

    String getExportLink() {
        List<String> containsWords = this.useContainsWordsFilter? this.containedWords : [];
        List<String> excludeWords = this.useExcludeWordsFilter? this.excludedWords : [];
        DateRange dateRange = this.useDateFilter? this.dateRange : null;

        return this._socnetoService.getJobPostsExportLink(this.job.id, containsWords, excludeWords, dateRange);
    }

    String prettyJson(dynamic obj) => JSON_ENCODER.convert(obj);

    void onRefresh() async => this._updateDisplayedPosts();

    void onPageChange(int page) async {
        this.paginator.currentPage = page;
        await this._updateDisplayedPosts();
    }

    void onRemoveDateFilter() {
        this.useDateFilter = false;
        this._updateDisplayedPosts();
    }

    void onRemoveContainsWordsFilter() {
        this.useContainsWordsFilter = false;
        this.containedWords.clear();
        this._updateDisplayedPosts();
    }

    void onRemoveExcludeWordsFilter() {
        this.useExcludeWordsFilter = false;
        this.excludedWords.clear();
        this._updateDisplayedPosts();
    }

    void onDateRangeChange(DatepickerComparison dateRange) {
        this.dateRange = dateRange?.range != null ? DateRange(dateRange.range.start, dateRange.range.end) : null;
        this._updateDisplayedPosts();
    }

    bool someFilterMissing() =>
        !useDateFilter || !useContainsWordsFilter || !useExcludeWordsFilter;

    bool canAddNewContainedWord() =>
        this.newContainedWord.isNotEmpty && !this.containedWords.contains(this.newContainedWord);

    void onAddContainedWord() {
        if (!this.canAddNewContainedWord()) {
            return;
        }

        this.containedWords.add(newContainedWord);
        this.newContainedWord = "";
        this._updateDisplayedPosts();
    }

    void onRemoveContainedWord(String word) {
        this.containedWords.removeWhere((x) => x == word);
        this._updateDisplayedPosts();
    }

    bool canAddNewExcludedWord() =>
        this.newExcludedWord.isNotEmpty && !this.excludedWords.contains(this.newExcludedWord);

    void onAddExcludedWord() {
        if (!this.canAddNewExcludedWord()) {
            return;
        }

        this.excludedWords.add(newExcludedWord);
        this.newExcludedWord = "";
        this._updateDisplayedPosts();
    }

    void onRemoveExcludedWord(String word) {
        this.excludedWords.removeWhere((x) => x == word);
        this._updateDisplayedPosts();
    }

    void _updateDisplayedPosts() async {
        this.loading = true;
        try {
            List<String> containsWords = this.useContainsWordsFilter? this.containedWords : [];
            List<String> excludeWords = this.useExcludeWordsFilter? this.excludedWords : [];
            DateRange dateRange = this.useDateFilter? this.dateRange : null;

            var paginatedPosts = await this._socnetoService.getJobPosts(job.id, paginator.currentPage,
                paginator.pageSize, containsWords, excludeWords, dateRange);
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