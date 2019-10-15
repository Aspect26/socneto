import 'dart:async';

import 'package:sw_project/src/models/CreateJobResponse.dart';
import 'package:sw_project/src/models/Credentials.dart';
import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/SocnetoComponentsResponse.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';

class SocnetoJobManagementService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6009";
  static const String API_PREFIX = "api";

  SocnetoJobManagementService() : super(API_URL, API_PREFIX);

  Future<List<SocnetoComponent>> getAvailableNetworks() async =>
      (await this.get<SocnetoComponentsResponse> ("components/networks", (result) => SocnetoComponentsResponse.fromMap(result))).components;

  Future<List<SocnetoAnalyser>> getAvailableAnalyzers() async =>
    (await this.get<SocnetoAnalysersResponse>("components/analysers", (result) => SocnetoAnalysersResponse.fromMap(result))).components;

  Future<String> submitNewJob(String query, List<SocnetoComponent> networks, List<SocnetoComponent> analyzers, TwitterCredentials twitterCredentials) async {
    var data = {
      "TopicQuery": query,
      "SelectedAnalysers": analyzers.map((analyzer) => analyzer.identifier).toList(),
      "SelectedNetworks": networks.map((network) => network.identifier).toList(),
    };

    if (twitterCredentials != null) {
      data.addAll({
        "TwitterCredentials": {
          "ApiKey": twitterCredentials.apiKey,
          "ApiKeySecret": twitterCredentials.apiSecretKey,
          "AccessToken": twitterCredentials.accessToken,
          "AccessTokenSecret": twitterCredentials.accessTokenSecret
        }
      });
    }

    return (await this.post<CreateJobResponse>(
        "job/submit", data, (result) =>
        CreateJobResponse.fromMap(result))).jobId;
  }

}