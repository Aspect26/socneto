import 'package:tuple/tuple.dart';

import 'Credentials.dart';
import 'SocnetoComponent.dart';

class JobSubmitRequest {
  final String jobName;
  final String query;
  final List<SocnetoComponent> acquirers;
  final List<SocnetoComponent> analyzers;
  final String language;
  final List<Tuple2<String, TwitterCredentials>> twitterCredentials;
  final List<Tuple2<String, RedditCredentials>> redditCredentials;

  JobSubmitRequest(this.jobName, this.query, this.acquirers, this.analyzers, this.language, this.twitterCredentials, this.redditCredentials);

  Map<String, dynamic> toMap() => {
    "jobName": jobName,
    "topicQuery": query,
    "selectedDataAnalysers": analyzers.map((analyzer) => analyzer.identifier).toList(),
    "selectedDataAcquirers": acquirers.map((acquirer) => acquirer.identifier).toList(),
    "language": language,
    "properties": this._createPropertiesField()
  };

  Map<String, dynamic> _createPropertiesField() {
    Map<String, dynamic> properties = {};

    this._addTwitterCredentialsFields(properties);
    this._addRedditCredentialsFields(properties);

    return properties;
  }

  void _addTwitterCredentialsFields(Map<String, dynamic> properties) {
    this.twitterCredentials.forEach((twitterCredentialsWithComponentId) {
      var componentId = twitterCredentialsWithComponentId.item1;
      var twitterCredentials = twitterCredentialsWithComponentId.item2;

      if (!properties.containsKey(componentId)) {
        properties[componentId] = {};
      }

      var twitterCredentialsFields =  this._createTwitterCredentialsFields(twitterCredentials);
      properties[componentId].putAll(twitterCredentialsFields);
    });
  }

  void _addRedditCredentialsFields(Map<String, dynamic> properties) {
    this.redditCredentials.forEach((redditCredentialsWithComponentId) {
      var componentId = redditCredentialsWithComponentId.item1;
      var redditCredentials = redditCredentialsWithComponentId.item2;

      if (!properties.containsKey(componentId)) {
        properties[componentId] = {};
      }

      var redditCredentialsFields = this._createRedditCredentialsFields(redditCredentials);
      properties[componentId].putAll(redditCredentialsFields);
    });
  }

  Map<String, dynamic> _createTwitterCredentialsFields(TwitterCredentials twitterCredentials) => {
    "TwitterApiKey": twitterCredentials.apiKey,
    "TwitterApiKeySecret": twitterCredentials.apiSecretKey,
    "TwitterAccessToken": twitterCredentials.accessToken,
    "TwitterAccessTokenSecret": twitterCredentials.accessTokenSecret
  };

  Map<String, dynamic> _createRedditCredentialsFields(RedditCredentials redditCredentials) => {
    "RedditAppId": redditCredentials.appId,
    "RedditAppSecret": redditCredentials.appSecret,
    "RedditRefreshToken": redditCredentials.refreshToken
  };

}