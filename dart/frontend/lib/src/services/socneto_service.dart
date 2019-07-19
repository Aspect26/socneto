import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/services/socneto_data_service.dart';
import 'package:sw_project/src/services/socneto_job_management_service.dart';

class SocnetoService {

  final _dataService = SocnetoDataService();
  final _job_management_service = SocnetoJobManagementService();

  Future<int> login(String username, String password) async {
    var result = await this._dataService.login(username, password);

    if (result != null) {
      this._dataService.setCredentials(username, password);
      this._job_management_service.setCredentials(username, password);
    }

    return result;
  }

  Future<Job> getJob(String jobId) async =>
      this._dataService.getJob(jobId);
  Future<List<Job>> getUserJobs(int userId) async =>
      this._dataService.getUserJobs(userId);
  Future<List<Post>> getJobPosts(String jobId) async =>
      this._dataService.getJobPosts(jobId);

  Future<List<SocnetoComponent>> getAvailableNetworks() async =>
      this._job_management_service.getAvailableNetworks();
  Future<List<SocnetoComponent>> getAvailableAnalyzers() async =>
      this._job_management_service.getAvailableAnalyzers();
  Future<String> submitNewJob(String query, List<SocnetoComponent> networks, List<SocnetoComponent> analyzers) async =>
      this._job_management_service.submitNewJob(query, networks, analyzers);

}