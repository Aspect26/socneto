import 'dart:async';

import 'package:sw_project/src/models/CreateJobResponse.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';

class SocnetoJobManagementService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6009";
  static const String API_PREFIX = "api";

  SocnetoJobManagementService() : super(API_URL, API_PREFIX);

  Future<List<SocnetoComponent>> getAvailableNetworks() async =>
      await this.getList<SocnetoComponent> ("components/networks", (result) => SocnetoComponent.fromMap(result));

  Future<List<SocnetoComponent>> getAvailableAnalyzers() async =>
      await this.getList<SocnetoComponent> ("components/analysers", (result) => SocnetoComponent.fromMap(result));

  Future<String> submitNewJob(String query, List<SocnetoComponent> networks, List<SocnetoComponent> analyzers) async {
    var data = {
      "topic_query": query,
      "selected_analyzers": analyzers.map((analyzer) => analyzer.identifier),
      "selected_networks": networks.map((network) => network.identifier),
    };

    return (await this.post<CreateJobResponse>(
        "job/submit", data, (result) =>
        CreateJobResponse.fromMap(result))).jobId;
  }

}