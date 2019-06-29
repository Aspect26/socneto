import 'package:sw_project/src/models/Post.dart';

class JobResult {
  final String inputQuery;
  final List<Post> posts;

  JobResult(this.inputQuery, this.posts);

  JobResult.fromMap(Map data) :
        inputQuery = data["inputQuery"],
        posts = data["posts"] != null? (data["posts"] as List<dynamic>).map((d) => Post.fromMap(d)).toList() : [];
}