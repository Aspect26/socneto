import 'package:sw_project/src/models/Post.dart';

class AnalyzedPost {
  final String jobId;
  final Post post;
  final dynamic analysis;

  AnalyzedPost(this.jobId, this.post, this.analysis);

  AnalyzedPost.fromMap(Map data) :
        jobId = data["jobId"],
        post = data["post"] != null? Post.fromMap(data["post"]) : null,
        analysis = data["analysis"];
}