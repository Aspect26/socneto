import 'dart:async';

import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/Success.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/socneto_data_service.dart';

class SocnetoMockDataService extends SocnetoDataService {

  static final DateTime now = DateTime.now();

  static final List<User> mockUsers = [
    User("admin"),
    User("aspect")
  ];

  static final List<Job> mockJobs = [
    Job("1", "Running job", DateTime.now(), true, null),
    Job("2", "Paused job", DateTime.now(), false, DateTime.fromMicrosecondsSinceEpoch(1550000000000000))
  ];

  static final List<SocnetoComponent> mockAcquirers = [
    SocnetoComponent("Twitter acq", ComponentType.DataAcquirer),
    SocnetoComponent("Reddit acq", ComponentType.DataAcquirer),
  ];

  static final List<SocnetoAnalyser> mockAnalysers = [
    SocnetoAnalyser("sentiment", ComponentType.DataAnalyser, []),
    SocnetoAnalyser("keywords", ComponentType.DataAnalyser, []),
    SocnetoAnalyser("magic", ComponentType.DataAnalyser, []),
  ];

  static final List<AnalyzedPost> mockAnalyzedPosts = [
    AnalyzedPost(mockJobs[0].id, Post("asd", "asfd", now.add(Duration(days: -1))), { "polarity": {"type": "number", "value": 1 } }),
    AnalyzedPost(mockJobs[0].id, Post("asd", "asfd", now.add(Duration(days: -2))), { "polarity": {"type": "number", "value": 0 } }),
    AnalyzedPost(mockJobs[0].id, Post("asd", "asfd", now.add(Duration(days: -3))), { "polarity": {"type": "number", "value": 1 } })
  ];

  static final List<ChartDefinition> mockCharts = [
    ChartDefinition(["post/postedAt", "analyses/sentiment/polarity", "analyses/sentiment/accuracy"], ChartType.Line)
  ];

  Future<User> login(String username, String password) async =>
    Future.value(mockUsers.firstWhere((user) => user.username == username));

  Future<Job> getJob(String jobId) async =>
    Future.value(mockJobs.firstWhere((job) => job.id == jobId));

  Future<List<Job>> getUserJobs(String username) async =>
    Future.value(mockJobs);

  Future<List<Post>> getJobPosts(String jobId) async =>
    Future.value([]);

  Future<List<List<List<dynamic>>>> getChartData(String jobId, ChartDefinition chartDefinition) async {
    return Future.value([
      [["2019-10-23T10:01:46.2458271+00:00", 1],    ["2019-10-23T10:00:46.2458271+00:00", 0],    ["2019-10-23T09:59:46.2458271+00:00", 0],    ["2019-10-23T09:58:46.2458271+00:00", 1],    ["2019-10-23T09:57:46.2458271+00:00", 0]],
      [["2019-10-23T10:01:46.2458271+00:00", 0.98], ["2019-10-23T10:00:46.2458271+00:00", 0.95], ["2019-10-23T09:59:46.2458271+00:00", 0.99], ["2019-10-23T09:58:46.2458271+00:00", 0.93], ["2019-10-23T09:57:46.2458271+00:00", 0.9]]
    ]);
  }

  Future<List<SocnetoComponent>> getAvailableAcquirers() async =>
    Future.value(mockAcquirers);

  Future<List<SocnetoAnalyser>> getAvailableAnalyzers() async =>
    Future.value(mockAnalysers);

  Future<List<ChartDefinition>> getJobChartDefinitions(String jobId) async =>
    Future.value(mockCharts);

  Future<Success> createJobChartDefinition(String jobId, ChartDefinition chartDefinition) async {
    var data = {
      "ChartType": chartDefinition.chartType.toString().split('.').last,
      "JsonDataPaths": chartDefinition.jsonDataPaths
    };
    return this.post<Success>("job/$jobId/charts/create", data, (result) => Success.fromMap(result));
  }

}