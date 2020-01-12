import 'package:sw_project/src/models/JmsJobResponse.dart';

import '../utils.dart';


class Job {
  final String id;
  final String name;
  JobStatusCode status;
  final DateTime startedAt;
  final DateTime finishedAt;

  Job(this.id, this.name, this.status, this.startedAt, this.finishedAt);

  Job.fromMap(Map data) :
        id = data["jobId"] ?? "",
        name = data["jobName"] ?? "Unnamed",
        status = getEnumByString(JobStatusCode.values, data["status"], JobStatusCode.Running),
        startedAt = data["startedAt"] != null ? DateTime.parse(data["startedAt"]) : null,
        finishedAt = data["finishedAt"] != null ? DateTime.parse(data["finishedAt"]) : null;

}
