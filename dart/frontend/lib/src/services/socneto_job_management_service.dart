import 'dart:async';

import 'package:sw_project/src/models/CreateJobResponse.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/SocnetoComponentsResponse.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';

class SocnetoJobManagementService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6009";
  static const String API_PREFIX = "api";

  SocnetoJobManagementService() : super(API_URL, API_PREFIX);

  Future<List<SocnetoComponent>> getAvailableNetworks() async =>
      (await this.get<SocnetoComponentsResponse> ("components/networks", (result) => SocnetoComponentsResponse.fromMap(result))).components;

  Future<List<SocnetoComponent>> getAvailableAnalyzers() async =>
      (await this.get<SocnetoComponentsResponse> ("components/analysers", (result) => SocnetoComponentsResponse.fromMap(result))).components;

  Future<String> submitNewJob(String query, List<SocnetoComponent> networks, List<SocnetoComponent> analyzers) async {
    var data = {
      "TopicQuery": query,
      "SelectedAnalysers": analyzers.map((analyzer) => analyzer.identifier).toList(),
      "SelectedNetworks": networks.map((network) => network.identifier).toList(),
    };

    return (await this.post<CreateJobResponse>(
        "job/submit", data, (result) =>
        CreateJobResponse.fromMap(result))).jobId;
  }

}