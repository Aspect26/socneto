class Job {
  final String id;
  final String name;
  final DateTime startedAt;
  final DateTime pausedAt;
  bool isRunning;

  Job(this.id, this.name, this.startedAt, this.isRunning, this.pausedAt);

  Job.fromMap(Map data) :
        id = data["jobId"] ?? "",
        name = data["jobName"] ?? "Unnamed",
        pausedAt = data["pausedAt"] != null ? DateTime.parse(data["pausedAt"]) : null,
        startedAt = data["startedAt"] != null ? DateTime.parse(data["startedAt"]) : null,
        isRunning = data["isRunning"] ?? true;
}
