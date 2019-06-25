import 'package:angular/angular.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/routes.dart';
import 'package:sw_project/src/services/socneto_service.dart';
import 'package:sw_project/src/utils.dart';


@Component(
  selector: 'jobs-list',
  directives: [
    routerDirectives,
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    NgFor,
    NgIf
  ],
  templateUrl: 'jobs_list_component.html',
  styleUrls: [
    'package:angular_components/css/mdc_web/card/mdc-card.scss.css',
    'jobs_list_component.css'
  ],
  encapsulation: ViewEncapsulation.None,
  exports: [RoutePaths, Routes],
)
class JobsListComponent implements AfterChanges {

  @Input()
  int userId;

  final SocnetoService _socnetoService;
  final Router _router;

  List<Job> jobs = [];
  Job selectedJob;

  JobsListComponent(this._socnetoService, this._router);

  @override
  void ngAfterChanges() async {
    await this._loadData();

    // TODO: this is not really a nice solution...
    this._setSelectedJob(this._router.current);
    this._router.onRouteActivated.listen((RouterState event) {
      this._setSelectedJob(event);
    });
  }

  void selectJob(Job job) {
    this._router.navigate(RoutePaths.jobDetail.toUrl(parameters: {"userId": "${this.userId}", "jobId": "${job.id}"}));
  }

  String getProcessingTime(Job job) {
    var fromTime = job.startedAt;
    var toTime = job.finishedAt ?? DateTime.now();
    var timeDiff = toTime.difference(fromTime);

    return getDurationString(timeDiff);
  }

  String getHumanReadableDate(DateTime dateTime) {
    var minute = "0${dateTime.minute}";
    minute = minute.substring(minute.length - 2);

    return "${dateTime.day}.${dateTime.month}.${dateTime.year} ${dateTime.hour}:$minute";
  }

  void onCreateNewJob(_) {
    this._router.navigate(RoutePaths.createJob.toUrl(parameters: {"userId": "${this.userId}"}));
  }

  void _loadData() async {
    try {
      this.jobs = await this._socnetoService.getUserJobs(this.userId);
      this.jobs.sort((a,b) => a.startedAt.compareTo(b.startedAt));
      this.jobs.sort((a, b) => a.finished? b.finished? 0 : 1 : b.finished? -1 : 0);
    } catch (e) {
      // TODO: some error?
    }
  }

  void _setSelectedJob(RouterState routerState) {
    if (routerState == null || !routerState.parameters.containsKey("jobId")) {
      this.selectedJob = null;
    }

    var selectedJobId = routerState.parameters["jobId"];
    try {
      this.selectedJob = this.jobs.firstWhere((job) => job.id == selectedJobId);
    } catch (error) {
      this.selectedJob = null;
    }
  }

}