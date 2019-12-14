import 'dart:async';

import 'package:sw_project/src/models/Credentials.dart';
import 'package:sw_project/src/models/JmsJobResponse.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/base/http_service_basic_auth_base.dart';

class SocnetoJobManagementService extends HttpServiceBasicAuthBase {

  static const String API_URL = "http://localhost:6009";
  static const String API_PREFIX = "api";

  SocnetoJobManagementService() : super(API_URL, API_PREFIX);

  Future<JobStatus> submitNewJob(String jobName, String query, List<SocnetoComponent> networks, List<SocnetoComponent> analyzers, String language, TwitterCredentials twitterCredentials, RedditCredentials redditCredentials) async {
    var data = {
      "jobName": jobName,
      "topicQuery": query,
      "selectedDataAnalysers": analyzers.map((analyzer) => analyzer.identifier).toList(),
      "selectedDataAcquirers": networks.map((network) => network.identifier).toList(),
      "language": language
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

    if (redditCredentials != null) {
      data.addAll({
        "RedditCredentials": {
          "appId": redditCredentials.appId,
          "appSecret": redditCredentials.appSecret,
          "refreshToken": redditCredentials.refreshToken
        }
      });
    }

    return (await this.post<JobStatus>(
        "job/submit", data, (result) =>
        JobStatus.fromMap(result)));
  }

  Future<JobStatus> stopJob(String jobId) async =>
      await this.get<JobStatus>("job/stop/$jobId", (result) => JobStatus.fromMap(result));

}