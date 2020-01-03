import 'dart:async';

import 'package:sw_project/src/models/AggregateAnalysisRequest.dart';
import 'package:sw_project/src/models/ArrayAnalysisRequest.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/Success.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';


class SocnetoDataService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6010";
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

  Future<List<List<List<dynamic>>>> getChartData(String jobId, ChartDefinition chartDefinition) async {
    var analyserId = chartDefinition.analysisDataPaths[0].analyserId;
    var propertyNames = chartDefinition.analysisDataPaths.map((dataPath) => dataPath.property.name).toList();
    if (chartDefinition.chartType == ChartType.Pie) {
      return await this._getAggregatedChartData(jobId, analyserId, propertyNames[0]);
    } else {
      return await this._getArrayChartData(jobId, analyserId, propertyNames);
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

    var returnValue = [values];
    return returnValue;
  }

  Future<List<List<List<dynamic>>>> _getArrayChartData(String jobId, String analyserId, List<String> propertyNames) async {
    ArrayAnalysisRequest request = ArrayAnalysisRequest(analyserId, propertyNames);
    Map<String, dynamic> result = await this.post<dynamic>("job/$jobId/array_analysis", request.toMap(), (result) => result);

    List<List<dynamic>> values = [];
    var dataPoints = result["data"];
    dataPoints.forEach((datum) => {
      values.add(datum)
    });

    var returnValue = [values];
    return returnValue;
  }

  Future<List<SocnetoComponent>> getAvailableAcquirers() async =>
      (await this.getList<SocnetoComponent> ("components/acquirers", (result) => SocnetoComponent.fromMap(result)));

  Future<List<SocnetoAnalyser>> getAvailableAnalyzers() async =>
      (await this.getList<SocnetoAnalyser>("components/analysers", (result) => SocnetoAnalyser.fromMap(result)));

  Future<List<ChartDefinition>> getJobChartDefinitions(String jobId) async =>
      await this.getList<ChartDefinition>("job/$jobId/charts", (result) => ChartDefinition.fromMap(result));

  Future<Success> createJobChartDefinition(String jobId, ChartDefinition chartDefinition) async {
    var body = {
      "chart_type": chartDefinition.chartType.toString().split('.').last,
      "analysis_data_paths": chartDefinition.analysisDataPaths.map((analysisDataPath) => {
        "analyser_component_id": analysisDataPath.analyserId,
        "analyser_property": {
          "identifier": analysisDataPath.property.name,
          "type": analysisDataPath.property.type.toString().split('.').last
        }
      }).toList(),
      "is_x_post_datetime": chartDefinition.isXDateTime
    };

    return this.post<Success>("job/$jobId/charts/create", body, (result) => Success.fromMap(result));
  }

}
