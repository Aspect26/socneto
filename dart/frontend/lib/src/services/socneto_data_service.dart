import 'dart:async';

import 'package:sw_project/src/models/JobResult.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/http_service_basic_auth_base.dart';

class SocnetoDataService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:5000";
  // static const String API_URL = "http://acheron.ms.mff.cuni.cz:39103";
  static const String API_PREFIX = "api";

  SocnetoDataService() : super(API_URL, API_PREFIX);

  Future<User> login(String username, String password) async {
  var data = { "username": username, "password": password };
    return await this.post<User>("user/login", data, (result) => User.fromMap(result));
  }

  Future<Job> getJob(String jobId) async =>
    await this.get<Job>("job/$jobId/status", (result) => Job.fromMap(result));

  Future<List<Job>> getUserJobs(int userId) async {
    return await this.getList<Job>(
        "user/$userId/jobs", (result) => Job.fromMap(result));
  }

  Future<List<Post>> getJobPosts(String jobId) async {
    var jobResult = await this.get<JobResult>("job/$jobId/result", (result) => JobResult.fromMap(result));
    return jobResult.posts;
  }

}