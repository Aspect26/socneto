import 'dart:io';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_list/create_job/create_job_modal.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_list/stop_job/stop_job_modal.dart';
import 'package:sw_project/src/components/shared/paginator/Paginator.dart';
import 'package:sw_project/src/components/shared/paginator/paginator_component.dart';
import 'package:sw_project/src/components/shared/platform_startup_info/platform_startup_info_component.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/JmsJobResponse.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/JobStatusCode.dart';
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
    PlatformStartupInfoComponent,
    PaginatorComponent,
    CreateJobModal,
    StopJobModal,
    NgFor,
    NgIf,
    NgClass
  ],
  templateUrl: 'job_list_component.html',
  styleUrls: [
    'package:angular_components/css/mdc_web/card/mdc-card.scss.css',
    'job_list_component.css'
  ],
  encapsulation: ViewEncapsulation.None,
  exports: [RoutePaths, Routes],
  // changeDetection: ChangeDetectionStrategy.OnPush
)
class JobListComponent implements AfterChanges {

  @ViewChild(CreateJobModal) CreateJobModal createJobModal;
  @ViewChild(StopJobModal) StopJobModal stopJobModal;
  @Input() String username;

  static final int PAGE_SIZE = 10;
  final SocnetoService _socnetoService;
  final Router _router;

  Paginator paginator = Paginator(0, 1, PAGE_SIZE);
  List<Job> jobs = [];
  List<Job> displayedJobs = [];
  Job selectedJob;
  bool isPlatformRunning = false;

  JobListComponent(this._socnetoService, this._router);

  @override
  void ngAfterChanges() async {
    await this._loadData();
    this._setSelectedJob(this._router.current);
  }

  int get runningJobs => jobs.where((job) => job.status == JobStatusCode.Running).length;
  JobStatusCode get runningJobStatus => JobStatusCode.Running;
  JobStatusCode get stoppedJobStatus => JobStatusCode.Stopped;
  String jobUrl(String jobId) => RoutePaths.jobDetail.toUrl(parameters: RouteParams.jobDetailParams(this.username, jobId));
  String guideUrl() => RoutePaths.workspaceHome.toUrl(parameters: RouteParams.workspaceParams(this.username));

  void onPlatformStarted() {
    this.isPlatformRunning = true;
    this._loadData();
  }

  void selectJob(String jobId) {
    this._router.navigate(RoutePaths.jobDetail.toUrl(parameters: RouteParams.jobDetailParams(this.username, jobId)));
  }

  void stopJob(Job job) async {
    this.stopJobModal.show(job);
  }

  String getProcessingTime(Job job) {
    var fromTime = job.startedAt;
    var toTime = job.finishedAt != null? job.finishedAt : DateTime.now();
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

  void onStopJobSubmit(JobStatus jobStatus) async {
    this.stopJobModal.close();
    this._setJobStatus(jobStatus);
  }

  void onCreateJobSubmit(JobStatus jobStatus) async {
    this.createJobModal.close();
    await this._loadData();
    this.selectJob(jobStatus.jobId);
  }

  void _loadData() async {
    try {
      this.jobs = await this._socnetoService.getUserJobs();
      this.paginator = Paginator(this.jobs.length, this.paginator.currentPage, PAGE_SIZE);
    } on HttpException catch (e) {
      this.jobs = [];
      this.paginator = Paginator(0, 1, PAGE_SIZE);
      this._onLoadDataError(e);
    }

    this.jobs.sort((a,b) => a.startedAt.compareTo(b.startedAt));
    this.jobs.sort((a, b) => a.status == JobStatusCode.Running? b.status == JobStatusCode.Running? 1 : 0 : b.status == JobStatusCode.Running? 0 : -1);
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

  void _setJobStatus(JobStatus jobStatus) {
    var job = this.jobs.firstWhere((job) => job.id == jobStatus.jobId, orElse: () => null);
    if (job != null) {
      job.status = jobStatus.status;
    }
  }

  void _updateDisplayedJobs() {
    final start = (this.paginator.currentPage - 1) * this.paginator.pageSize;
    var end = (this.paginator.currentPage) * this.paginator.pageSize;

    if (end > this.jobs.length) {
      end = this.jobs.length;
    }

    this.displayedJobs = this.jobs.sublist(start, end);
  }

  void _onLoadDataError(HttpException error) {
    if (error.statusCode == 401) {
      this._router.navigate(RoutePaths.notAuthorized.toUrl());
    } else {
      Toastr.httpError(error);
      return;
    }

    throw error;
  }

}
