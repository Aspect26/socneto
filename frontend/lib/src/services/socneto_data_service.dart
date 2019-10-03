import 'dart:async';

import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Success.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';

class SocnetoDataService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6010";
  // static const String API_URL = "http://acheron.ms.mff.cuni.cz:39103";
  static const String API_PREFIX = "api";

  SocnetoDataService() : super(API_URL, API_PREFIX);

  Future<User> login(String username, String password) async {
    var data = { "username": username, "password": password };
    return await this.post<User>("user/login", data, (result) => User.fromMap(result));
  }

  Future<Job> getJob(String jobId) async =>
    await this.get<Job>("job/$jobId/status", (result) => Job.fromMap(result));

  Future<List<Job>> getUserJobs(String username) async =>
    await this.getList<Job>("user/$username/jobs", (result) => Job.fromMap(result));

  Future<List<Post>> getJobPosts(String jobId) async =>
    await this.getList<Post>("job/$jobId/posts", (result) => Post.fromMap(result));

  Future<List<AnalyzedPost>> getJobAnalysis(String jobId) async =>
    await this.getList<AnalyzedPost>("job/$jobId/analysis", (result) => AnalyzedPost.fromMap(result));

  Future<List<ChartDefinition>> getJobChartDefinitions(String jobId) async =>
    // await this.getList<ChartDefinition>("job/$jobId/charts", (result) => ChartDefinition.fromMap(result));
    // TODO: mock here
    Future.value([
      ChartDefinition(["DataAnalyzer_Mock.polarity"], ChartType.Line),
      ChartDefinition(["DataAnalyzer_Mock.polarity", "DataAnalyzer_Mock.accuracy"], ChartType.Line)
    ]);

  Future<Success> createJobChartDefinition(String jobId, ChartDefinition chartDefinition) async {
    var data = {
      "ChartType": chartDefinition.chartType.toString().split('.').last,
      "DataJsonPath": chartDefinition.jsonDataPaths
    };
    return this.post<Success>("job/$jobId/charts/create", data, (result) => Success.fromMap(result));
  }

}