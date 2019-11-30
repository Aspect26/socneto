class ArrayAnalysisRequest {
  final String analyserId;
  final List<String> analysisProperties;

  ArrayAnalysisRequest(this.analyserId, this.analysisProperties);

  Map<String, dynamic> toMap() => {
    'analyserId': this.analyserId,
    'analysisProperties': this.analysisProperties
  };

}
