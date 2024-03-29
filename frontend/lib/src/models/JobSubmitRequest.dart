import 'SocnetoComponent.dart';

class JobSubmitRequest {
  final String jobName;
  final String query;
  final List<SocnetoComponent> acquirers;
  final List<SocnetoComponent> analyzers;
  final String language;
  final Map<String, Map<String, String>> attributes;

  JobSubmitRequest(this.jobName, this.query, this.acquirers, this.analyzers, this.language, this.attributes);

  Map<String, dynamic> toMap() => {
    "job_name": this.jobName,
    "topic_query": this.query,
    "selected_acquirers": this.acquirers.map((acquirer) => acquirer.identifier).toList(),
    "selected_analysers": this.analyzers.map((analyzer) => analyzer.identifier).toList(),
    "language": this.language,
    "attributes": this.attributes
  };
}