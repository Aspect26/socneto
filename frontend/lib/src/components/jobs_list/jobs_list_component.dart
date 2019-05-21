import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/services/socneto_service.dart';

import '../../utils.dart';

@Component(
  selector: 'jobs-list',
  directives: [
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    NgFor,
    NgIf
  ],
  providers: [
    ClassProvider(SocnetoService)
  ],
  templateUrl: 'jobs_list_component.html',
  styleUrls: [
    'package:angular_components/css/mdc_web/card/mdc-card.scss.css',
    'jobs_list_component.css'
  ],
  encapsulation: ViewEncapsulation.None
)
class TasksListComponent implements OnInit {

  final SocnetoService _socnetoService;

  List<Job> tasks = [];
  var errorMessage = "";

  TasksListComponent(this._socnetoService);

  final _selectRequest = StreamController<Job>();
  @Output() Stream<Job> get selected => _selectRequest.stream;

  @override
  void ngOnInit() {
    this._loadData();
  }

  void selectTask(Job job) {
    this._selectRequest.add(job);
  }

  String getProcessingTime(Job job) {
    var fromTime = job.startedAt;
    var toTime = job.finishedAt ?? DateTime.now();
    var timeDiff = toTime.difference(fromTime);

    return getDurationString(timeDiff);
  }

  void _loadData() async {
    try {
      this.tasks = await this._socnetoService.getUserJobs(2);
      this.tasks.sort((a,b) => a.startedAt.compareTo(b.startedAt));
      this.tasks.sort((a, b) => a.finished? b.finished? 0 : 1 : b.finished? -1 : 0);
    } catch (e) {
      this.errorMessage = e.toString();
    }
  }

}