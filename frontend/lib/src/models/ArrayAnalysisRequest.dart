class ArrayAnalysisRequest {
  final String analyserId;
  final List<String> analysisProperties;

  ArrayAnalysisRequest(this.analyserId, this.analysisProperties);

  Map<String, dynamic> toMap() => {
    'analyser_id': this.analyserId,
    'analysis_properties': this.analysisProperties
  };

}
