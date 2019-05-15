class Job {
  final String id;
  final bool finished;
  String name;
  DateTime startedAt;
  DateTime finishedAt;

  Job(this.id, this.name, this.startedAt, this.finished, this.finishedAt);

  Job.fromMap(Map data) :
        id = data["jobId"] ?? "",
        name = data["jobName"] ?? "Unnamed",
        finished = data["hasFinished"] ?? false,
        startedAt = data["startedAt"] != null ? DateTime.parse(data["startedAt"]) : null,
        finishedAt = data["finishedAt"] != null ? DateTime.parse(data["finishedAt"]) : null;
}