import '../utils.dart';
import 'JobStatusCode.dart';


class Job {
  final String id;
  final String name;
  final String topicQuery;
  JobStatusCode status;
  final DateTime startedAt;
  final DateTime finishedAt;

  Job(this.id, this.name, this.topicQuery, this.status, this.startedAt, this.finishedAt);

  Job.fromMap(Map data) :
        id = data["job_id"] ?? "",
        name = data["job_name"] ?? "Unnamed",
        topicQuery = data["topic_query"] ?? "",
        status = getEnumByString(JobStatusCode.values, data["status"], JobStatusCode.Running),
        startedAt = data["started_at"] != null ? DateTime.parse(data["started_at"]) : null,
        finishedAt = data["finished_at"] != null ? DateTime.parse(data["finished_at"]) : null;

}
