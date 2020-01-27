class ArrayAnalysisRequest {
  final String analyserId;
  final List<String> analysisProperties;
  final bool isXPostDate;

  ArrayAnalysisRequest(this.analyserId, this.analysisProperties, this.isXPostDate);

  Map<String, dynamic> toMap() => {
    'analyser_id': this.analyserId,
    'analysis_properties': this.analysisProperties,
    'is_x_post_date': this.isXPostDate,
  };

}
