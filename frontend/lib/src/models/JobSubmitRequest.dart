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
    "job_name": jobName,
    "topic_query": query,
    "selected_acquirers": acquirers.map((acquirer) => acquirer.identifier).toList(),
    "selected_analysers": analyzers.map((analyzer) => analyzer.identifier).toList(),
    "language": language,
    "credentials": this._createCredentialsField()
  };

  Map<String, dynamic> _createCredentialsField() {
    Map<String, dynamic> credentials = {};

    this.credentials.forEach((acquirerCredentials) {
      var acquirerId = acquirerCredentials.item1;
      var twitterCredentials = acquirerCredentials.item2;
      var redditCredentials = acquirerCredentials.item3;

      credentials[acquirerId] = {
        "twitter": twitterCredentials != null? twitterCredentials.toJson() : null,
        "reddit": redditCredentials != null? redditCredentials.toJson() : null
      };
    });

    return credentials;
  }
}