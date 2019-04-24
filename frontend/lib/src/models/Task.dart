class Task {
  final int id;
  final bool finished;
  String name;
  DateTime startedAt;
  DateTime finishedAt;

  Task(this.id, this.name, this.startedAt, this.finished, this.finishedAt);
}