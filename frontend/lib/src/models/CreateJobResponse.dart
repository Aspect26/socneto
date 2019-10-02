class CreateJobResponse {
  final String jobId;

  CreateJobResponse(this.jobId);

  CreateJobResponse.fromMap(Map data) :
        jobId = data["jobId"] ?? "";
}