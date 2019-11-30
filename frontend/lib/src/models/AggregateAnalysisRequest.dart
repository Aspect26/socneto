class AggregateAnalysisRequest {
  final String analyserId;
  final String analysisProperty;

  AggregateAnalysisRequest(this.analyserId, this.analysisProperty);

  Map<String, dynamic> toMap() => {
    'analyserId': this.analyserId,
    'analysisProperty': this.analysisProperty
  };

}
