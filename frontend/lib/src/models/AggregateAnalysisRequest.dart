class AggregateAnalysisRequest {
  final String analyserId;
  final String analysisProperty;

  AggregateAnalysisRequest(this.analyserId, this.analysisProperty);

  Map<String, dynamic> toMap() => {
    'analyser_id': this.analyserId,
    'analysis_property': this.analysisProperty
  };

}
