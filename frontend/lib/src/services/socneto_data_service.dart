import 'dart:async';

import 'package:angular_components/angular_components.dart';
import 'package:sw_project/src/config.dart';
import 'package:sw_project/src/models/AggregateAnalysisRequest.dart';
import 'package:sw_project/src/models/ArrayAnalysisRequest.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/JmsJobResponse.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/JobSubmitRequest.dart';
import 'package:sw_project/src/models/PaginatedPosts.dart';
import 'package:sw_project/src/models/PlatformStatus.dart';
import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/Success.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';
import 'package:tuple/tuple.dart';


class SocnetoDataService extends HttpServiceBasicAuthBase {

  static String API_URL = Config.backendHost ?? "http://localhost:6010";
  static const String API_PREFIX = "api";

  SocnetoDataService() : super(API_URL, API_PREFIX);

  Future<PlatformStatus> getPlatformStatus() async =>
      await this.get<PlatformStatus>("platform_status", (result) => PlatformStatus.fromMap(result));

  Future<User> login(String username, String password) async {
    var data = { "username": username, "password": password };
    return await this.post<User>("user/login", data, (result) => User.fromMap(result));
  }

  Future<JobStatus> submitNewJob(String jobName, String query, List<SocnetoComponent> acquirers,
      List<SocnetoComponent> analyzers, String language,
      Map<String, Map<String, String>> attributes)
  async {
    var request = JobSubmitRequest(jobName, query, acquirers, analyzers, language, attributes);
    return (await this.post<JobStatus>("job/create", request.toMap(), (result) => JobStatus.fromMap(result)));
  }

  Future<JobStatus> stopJob(String jobId) async =>
      await this.get<JobStatus>("job/$jobId/stop", (result) => JobStatus.fromMap(result));

  Future<Job> getJob(String jobId) async =>
      await this.get<Job>("job/$jobId/status", (result) => Job.fromMap(result));

  Future<List<Job>> getUserJobs() async =>
      await this.getList<Job>("job/all", (result) => Job.fromMap(result));

  Future<PaginatedPosts> getJobPosts(String jobId, int page, int pageSize, List<String> containsWords, List<String> excludeWords, DateRange dateRange) async {
    var path = "job/$jobId/posts?page=$page&page_size=$pageSize"
        "${containsWords.map((word) => "&contains_words=$word").toList().join()}"
        "${excludeWords.map((word) => "&exclude_words=$word").toList().join()}"
        "${dateRange != null? "&from=${dateRange.start.toString()}&to=${dateRange.end.toString()}" : ""}";
    return await this.get<PaginatedPosts>(path, (result) => PaginatedPosts.fromMap(result));
  }

  String getJobPostsExportLink(String jobId, List<String> containsWords, List<String> excludeWords, DateRange dateRange) {
    var path = "job/$jobId/posts/export?"
        "${containsWords.map((word) => "&contains_words=$word").toList().join()}"
        "${excludeWords.map((word) => "&exclude_words=$word").toList().join()}"
        "${dateRange != null? "&from=${dateRange.start.toString()}&to=${dateRange.end.toString()}" : ""}";

    return this.getFullApiCallPath(path);
  }

  Future<List<List<List<dynamic>>>> getChartData(String jobId, ChartDefinition chartDefinition) async {
    var analyserId = chartDefinition.analysisDataPaths[0].analyserId;
    var propertyNames = chartDefinition.analysisDataPaths.map((dataPath) => dataPath.property).toList();
    if (chartDefinition.chartType == ChartType.Pie) {
      return await this._getAggregatedChartData(jobId, analyserId, propertyNames[0]);
    } else {
      return await this._getArrayChartData(jobId, analyserId, propertyNames, chartDefinition.isXDateTime);
    }
  }

  Future<List<List<List<dynamic>>>> _getAggregatedChartData(String jobId, String analyserId, String propertyName) async {
    AggregateAnalysisRequest request = AggregateAnalysisRequest(analyserId, propertyName);
    Map<String, dynamic> result = await this.post<dynamic>("job/$jobId/aggregation_analysis", request.toMap(), (result) => result);

    List<List<dynamic>> values = [];
    var aggregations = result["aggregations"];
    aggregations.forEach((key, value) => {
      values.add([key, value])
    });

    return values.isEmpty? [] : [values];
  }

  Future<List<List<List<dynamic>>>> _getArrayChartData(String jobId, String analyserId, List<String> propertyNames, bool isXPostDate) async {
    ArrayAnalysisRequest request = ArrayAnalysisRequest(analyserId, propertyNames, isXPostDate);
    Map<String, dynamic> result = await this.post<dynamic>("job/$jobId/array_analysis", request.toMap(), (result) => result);

    List<List<dynamic>> values = [];
    var dataPoints = result["data"];
    dataPoints.forEach((datum) => {
      values.add(datum)
    });

    return values.isEmpty? [] : [values];
  }

  Future<List<SocnetoComponent>> getAvailableAcquirers() async =>
      (await this.getList<SocnetoComponent> ("components/acquirers", (result) => SocnetoComponent.fromMap(result)));

  Future<List<SocnetoAnalyser>> getAvailableAnalyzers() async =>
      (await this.getList<SocnetoAnalyser>("components/analysers", (result) => SocnetoAnalyser.fromMap(result)));

  Future<List<ChartDefinition>> getJobChartDefinitions(String jobId) async =>
      await this.getList<ChartDefinition>("charts/$jobId", (result) => ChartDefinition.fromMap(result));

  Future<Success> createJobChartDefinition(String jobId, ChartDefinition chartDefinition) async {
    var body = {
      "title": chartDefinition.title,
      "chart_type": chartDefinition.chartType.toString().split('.').last,
      "analysis_data_paths": chartDefinition.analysisDataPaths.map((analysisDataPath) => {
        "analyser_component_id": analysisDataPath.analyserId,
        "analyser_property": analysisDataPath.property,
      }).toList(),
      "is_x_post_datetime": chartDefinition.isXDateTime
    };

    return this.post<Success>("charts/$jobId/create", body, (result) => Success.fromMap(result));
  }

}
