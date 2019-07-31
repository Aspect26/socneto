import 'package:sw_project/src/models/Post.dart';

class AnalyzedPost {
  final String jobId;
  final Post post;
  final dynamic analysis;

  AnalyzedPost(this.jobId, this.post, this.analysis);

  AnalyzedPost.fromMap(Map data) :
        jobId = data["jobId"],
        post = Post.fromMap(data["post"]),
        analysis = data["analysis"];
}