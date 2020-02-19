class PostsFrequencyAnalysisRequest {
  final String jobId;

  PostsFrequencyAnalysisRequest(this.jobId);

  Map<String, dynamic> toMap() => {
    "job_id": this.jobId
  };
}

class ArrayAnalysisRequest {
  final String analyserId;
  final List<String> analysisProperties;
  final bool isXPostDate;
  final int pageSize;
  final int page;

  ArrayAnalysisRequest(this.analyserId, this.analysisProperties, this.isXPostDate, this.pageSize, this.page);

  Map<String, dynamic> toMap() => {
    'analyser_id': this.analyserId,
    'analysis_properties': this.analysisProperties,
    'is_x_post_date': this.isXPostDate,
    'page_size': this.pageSize,
    'page': this.page,
  };

}

class AggregateAnalysisRequest {
  final String analyserId;
  final String analysisProperty;

  AggregateAnalysisRequest(this.analyserId, this.analysisProperty);

  Map<String, dynamic> toMap() => {
    'analyser_id': this.analyserId,
    'analysis_property': this.analysisProperty
  };

}

