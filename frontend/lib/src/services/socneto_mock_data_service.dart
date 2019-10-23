import 'dart:async';

import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/Success.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';

class SocnetoMockDataService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6010";
  static const String API_PREFIX = "api";
  
  static final List<Job> mockJobs = [
    Job("1", "Running job", DateTime.now(), true, null),
    Job("2", "Paused job", DateTime.now(), false, DateTime.fromMicrosecondsSinceEpoch(1550000000000000))
  ];

  SocnetoMockDataService() : super(API_URL, API_PREFIX);

  Future<User> login(String username, String password) async {
    var data = { "username": username, "password": password };
    return await this.post<User>("user/login", data, (result) => User.fromMap(result));
  }

  Future<Job> getJob(String jobId) async =>
    Future.value(mockJobs.firstWhere((job) => job.id == jobId));

  Future<List<Job>> getUserJobs(String username) async =>
    Future.value(mockJobs);

  Future<List<Post>> getJobPosts(String jobId) async =>
    await this.getList<Post>("job/$jobId/posts", (result) => Post.fromMap(result));

  Future<List<AnalyzedPost>> getJobAnalysis(String jobId) async =>
    await this.getList<AnalyzedPost>("job/$jobId/analysis", (result) => AnalyzedPost.fromMap(result));

  Future<List<SocnetoComponent>> getAvailableNetworks() async =>
      (await this.getList<SocnetoComponent> ("components/acquirers", (result) => SocnetoComponent.fromMap(result)));

  Future<List<SocnetoAnalyser>> getAvailableAnalyzers() async =>
      (await this.getList<SocnetoAnalyser>("components/analysers", (result) => SocnetoAnalyser.fromMap(result)));

  Future<List<ChartDefinition>> getJobChartDefinitions(String jobId) async =>
    await this.getList<ChartDefinition>("job/$jobId/charts", (result) => ChartDefinition.fromMap(result));

  Future<Success> createJobChartDefinition(String jobId, ChartDefinition chartDefinition) async {
    var data = {
      "ChartType": chartDefinition.chartType.toString().split('.').last,
      "JsonDataPaths": chartDefinition.jsonDataPaths
    };
    return this.post<Success>("job/$jobId/charts/create", data, (result) => Success.fromMap(result));
  }

}