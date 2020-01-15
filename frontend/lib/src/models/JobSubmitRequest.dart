import 'package:tuple/tuple.dart';

import 'Credentials.dart';
import 'SocnetoComponent.dart';

class JobSubmitRequest {
  final String jobName;
  final String query;
  final List<SocnetoComponent> acquirers;
  final List<SocnetoComponent> analyzers;
  final String language;
  final List<Tuple3<String, TwitterCredentials, RedditCredentials>> credentials;

  JobSubmitRequest(this.jobName, this.query, this.acquirers, this.analyzers, this.language, this.credentials);

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

    this._addCredentialsFields(properties);

    return properties;
  }

  void _addCredentialsFields(Map<String, dynamic> properties) {
    this.acquirers.forEach((dataAcquirer) {
      var dataAcquirerId = dataAcquirer.identifier;

//      TODO: uncomment this when implemented in JMS
//      if (!properties.containsKey(componentId)) {
//        properties[componentId] = Map();
//      }

      var acquirerCredentials = this.credentials.firstWhere((credentialsWithComponentId) => credentialsWithComponentId.item1 == dataAcquirerId, orElse: () => null);
      var twitterCredentialsFields =  this._createTwitterCredentialsFields(acquirerCredentials?.item2);
      var redditCredentialsFields = this._createRedditCredentialsFields(acquirerCredentials?.item3);
//      properties[componentId].addAll(twitterCredentialsFields);
//      properties[componentId].addAll(redditCredentialsFields);
      properties.addAll(twitterCredentialsFields);
      properties.addAll(redditCredentialsFields);
    });
  }

  Map<String, dynamic> _createTwitterCredentialsFields(TwitterCredentials twitterCredentials) => {
    // TODO: this should be done on backend
    "ApiKey": twitterCredentials == null? "b0sXpwv0k4eJYmLaBZvSfNyxk" : twitterCredentials.apiKey,
    "ApiSecretKey": twitterCredentials == null? "KK7aRtfR8XGFqaz2Ua1vDgYftQ01AqlUYXKkqrpWoVxR8FC0p7" : twitterCredentials.apiSecretKey,
    "AccessToken": twitterCredentials == null? "1178262834595209216-2TTiiia1yOVd2UHHH07dBd7KDBN7pZ" : twitterCredentials.accessToken,
    "AccessTokenSecret": twitterCredentials == null? "fhAw5o5IeTTc33mGtpFnRKGMpbEA6xcsnkIrFZBNZXq4c" : twitterCredentials.accessTokenSecret
  };

  Map<String, dynamic> _createRedditCredentialsFields(RedditCredentials redditCredentials) => {
    "RedditAppId": redditCredentials == null? "" : redditCredentials.appId,
    "RedditAppSecret": redditCredentials == null? "" : redditCredentials.appSecret,
    "RedditRefreshToken": redditCredentials == null? "" : redditCredentials.refreshToken
  };

}