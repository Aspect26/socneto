import '../utils.dart';

class JobStatus {

  final JobStatusCode status;
  final String jobId;

  JobStatus(this.status, this.jobId);

  JobStatus.fromMap(Map data) :
        jobId = data["jobId"],
        status = getEnumByString(JobStatusCode.values, data["status"], JobStatusCode.Running);

}

enum JobStatusCode {
  Running,
  Stopped
}
