import 'Post.dart';

class Task {
  final int id;
  final bool finished;
  String name;
  DateTime startedAt;
  DateTime finishedAt;
  List<Post> posts;

  Task(this.id, this.name, this.startedAt, this.finished, this.finishedAt, this.posts);

  Task.fromMap(Map data) :
        id = data["id"],
        finished = data["finished"],
        name = data["name"],
        startedAt = data["startedAt"] != null ? DateTime.parse(data["startedAt"]) : null,
        finishedAt = data["finishedAt"] != null ? DateTime.parse(data["finishedAt"]) : null,
        posts = data["posts"] != null ? (data["posts"] as List).map((p) => Post.fromMap(p)) : [];
}