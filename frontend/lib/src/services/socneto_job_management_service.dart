import 'dart:async';

import 'package:sw_project/src/models/Credentials.dart';
import 'package:sw_project/src/models/JmsJobResponse.dart';
import 'package:sw_project/src/models/JobSubmitRequest.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';
import 'package:tuple/tuple.dart';

class SocnetoJobManagementService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6009";
  static const String API_PREFIX = "api";

  SocnetoJobManagementService() : super(API_URL, API_PREFIX);

  Future<JobStatus> submitNewJob(
      String jobName,
      String query,
      List<SocnetoComponent> acquirers,
      List<SocnetoComponent> analyzers,
      String language,
      List<Tuple3<String, TwitterCredentials, RedditCredentials>> credentials)
  async {
    var request = JobSubmitRequest(jobName, query, acquirers, analyzers, language, credentials);
    return (await this.post<JobStatus>("job/submit", request.toMap(), (result) => JobStatus.fromMap(result)));
  }

  Future<JobStatus> stopJob(String jobId) async =>
      await this.get<JobStatus>("job/stop/$jobId", (result) => JobStatus.fromMap(result));

}