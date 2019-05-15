import 'dart:async';

import 'package:sw_project/src/models/JobResult.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/services/http_service_base.dart';

class SocnetoService extends HttpServiceBase {

  static const String API_URL = "http://acheron.ms.mff.cuni.cz:39103";
  static const String API_PREFIX = "api";

  SocnetoService() : super(API_URL, API_PREFIX);

  Future<List<Job>> getUserJobs(int userId) async {
    return await this.getList<Job>("user/$userId/jobs", (n) => Job.fromMap(n));
  }

  Future<List<Post>> getJobPosts(String jobId) async {
    var jobResult = await this.get<JobResult>("job/$jobId/result", (n) => JobResult.fromMap(n));
    return jobResult.posts;
  }

}