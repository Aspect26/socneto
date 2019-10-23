import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_list/create_job_modal.dart';
import 'package:sw_project/src/components/shared/paginator/Paginator.dart';
import 'package:sw_project/src/components/shared/paginator/paginator_component.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';
import 'package:sw_project/src/utils.dart';


@Component(
  selector: 'job-list',
  directives: [
    routerDirectives,
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    MaterialButtonComponent,
    PaginatorComponent,
    CreateJobModal,
    NgFor,
    NgIf
  ],
  templateUrl: 'job_list_component.html',
  styleUrls: [
    'package:angular_components/css/mdc_web/card/mdc-card.scss.css',
    'job_list_component.css'
  ],
  encapsulation: ViewEncapsulation.None,
  exports: [RoutePaths, Routes],
)
class JobListComponent implements AfterChanges {

  @ViewChild(CreateJobModal) CreateJobModal createJobModal;
  @Input() String username;

  static final int PAGE_SIZE = 10;
  final SocnetoService _socnetoService;
  final Router _router;

  Paginator paginator = Paginator(0, 0, PAGE_SIZE);
  List<Job> jobs = [];
  List<Job> displayedJobs = [];
  Job selectedJob;

  JobListComponent(this._socnetoService, this._router);

  @override
  void ngAfterChanges() async {
    await this._loadData();

    // TODO: this is not really a nice solution...
    this._setSelectedJob(this._router.current);
    this._router.onRouteActivated.listen((RouterState event) {
      this._setSelectedJob(event);
    });
  }

  int get runningJobs => jobs.where((job) => job.isRunning).length;

  void selectJob(String jobId) {
    this._router.navigate(RoutePaths.jobDetail.toUrl(parameters: RouteParams.jobDetailParams(this.username, jobId)));
  }

  void pauseJob(Job job) {
    // TODO: do this on backend/jms ofc
    job.isRunning = false;
  }

  void startJob(Job job) {
    // TODO: do this on backend/jms ofc
    job.isRunning = true;
  }

  String getProcessingTime(Job job) {
    var fromTime = job.startedAt;
    var toTime = DateTime.now();
    var timeDiff = toTime.difference(fromTime);

    return getDurationString(timeDiff);
  }

  String getHumanReadableDate(DateTime dateTime) {
    var minute = "0${dateTime.minute}";
    minute = minute.substring(minute.length - 2);

    return "${dateTime.day}.${dateTime.month}.${dateTime.year} ${dateTime.hour}:$minute";
  }

  void onCreateNewJob(_) {
    this.createJobModal.show();
  }

  void onPageChange(int page) {
    this.paginator.currentPage = page;
    this._updateDisplayedJobs();
  }

  void onCreateJobSubmit(String newJobId) async {
    this.createJobModal.close();
    await this._loadData();
    this.selectJob(newJobId);
  }

  void _loadData() async {
    try {
      this.jobs = await this._socnetoService.getUserJobs(this.username);
      this.paginator = Paginator(this.jobs.length, this.paginator.currentPage, PAGE_SIZE);
    } on HttpException catch (e) {
      this.jobs = [];
      this.paginator = Paginator(0, 0, PAGE_SIZE);
      this._onLoadDataError(e);
    }

    this.jobs.sort((a,b) => a.startedAt.compareTo(b.startedAt));
    this.jobs.sort((a, b) => a.isRunning? b.isRunning? 1 : 0 : b.isRunning? 0 : -1);
    this._updateDisplayedJobs();
  }

  void _setSelectedJob(RouterState routerState) {
    if (routerState == null || !routerState.parameters.containsKey(RouteParams.jobDetailJobId)) {
      this.selectedJob = null;
    }

    var selectedJobId = routerState.parameters[RouteParams.jobDetailJobId];
    try {
      this.selectedJob = this.jobs.firstWhere((job) => job.id == selectedJobId);
    } catch (error) {
      this.selectedJob = null;
    }
  }

  void _updateDisplayedJobs() {
    final start = this.paginator.currentPage * this.paginator.pageSize;
    var end = (this.paginator.currentPage + 1) * this.paginator.pageSize;

    if (end > this.jobs.length) {
      end = this.jobs.length;
    }

    this.displayedJobs = this.jobs.sublist(start, end);
  }

  void _onLoadDataError(HttpException error) {
    if (error.statusCode == 401) {
      this._router.navigate(RoutePaths.notAuthorized.toUrl());
    }

    throw error;
  }

}