import 'package:sw_project/src/models/Post.dart';

class AnalyzedPost {
  final String jobId;
  final Post post;
  final dynamic analyses;

  AnalyzedPost(this.jobId, this.post, this.analyses);

  AnalyzedPost.fromMap(Map data) :
        jobId = data["jobId"],
        post = Post.fromMap(data["postDto"]),
        analyses = data["analyses"];
}