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
    "attributes": this._createPropertiesField()
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

      // TODO: uncomment this when implemented in JMS
//      if (!properties.containsKey(componentId)) {
//        properties[componentId] = Map();
//      }

      var twitterCredentialsFields =  this._createTwitterCredentialsFields(twitterCredentials);
      //properties[componentId].addAll(twitterCredentialsFields);
      properties.addAll(twitterCredentialsFields);
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
    // TODO: uncomment me
    "ApiKey": "b0sXpwv0k4eJYmLaBZvSfNyxk",  //twitterCredentials.apiKey,
    "ApiSecretKey": "KK7aRtfR8XGFqaz2Ua1vDgYftQ01AqlUYXKkqrpWoVxR8FC0p7", // twitterCredentials.apiSecretKey,
    "AccessToken": "1178262834595209216-2TTiiia1yOVd2UHHH07dBd7KDBN7pZ", //twitterCredentials.accessToken,
    "AccessTokenSecret": "fhAw5o5IeTTc33mGtpFnRKGMpbEA6xcsnkIrFZBNZXq4c", //twitterCredentials.accessTokenSecret
  };

  Map<String, dynamic> _createRedditCredentialsFields(RedditCredentials redditCredentials) => {
    "RedditAppId": redditCredentials.appId,
    "RedditAppSecret": redditCredentials.appSecret,
    "RedditRefreshToken": redditCredentials.refreshToken
  };

}