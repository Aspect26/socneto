import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/JmsJobResponse.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/PaginatedAnalyzedPosts.dart';
import 'package:sw_project/src/models/PlatformStatus.dart';
import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/Success.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/local_storage_service.dart';
import 'package:sw_project/src/services/socneto_data_service.dart';
import 'package:sw_project/src/services/socneto_mock_data_service.dart';
import 'package:tuple/tuple.dart';


class SocnetoService {

//  final _dataService = SocnetoMockDataService();
  final _dataService = SocnetoDataService();
  final _local_storage_service = LocalStorageService();

  User tryLoginFromLocalStorage() {
    var userData = this._local_storage_service.loadUsernameToken();
    if (userData != null) {
      var username = userData[0];
      var authToken = userData[1];
      this._dataService.setToken(authToken);
      return User(username);
    } else {
      return null;
    }
  }

  Future<User> login(String username, String password) async {
    var result = await this._dataService.login(username, password);

    if (result != null) {
      this._dataService.setCredentials(username, password);
      this._local_storage_service.storeToken(username, this._dataService.getAuthToken());
    }

    return result;
  }

  Future<PlatformStatus> getPlatformStatus() async =>
      await this._dataService.getPlatformStatus();
  Future<Job> getJob(String jobId) async =>
      await this._dataService.getJob(jobId);
  Future<List<Job>> getUserJobs() async =>
      await this._dataService.getUserJobs();
  Future<PaginatedAnalyzedPosts> getJobPosts(String jobId, int page, int pageSize) async =>
      await this._dataService.getJobPosts(jobId, page, pageSize);
  Future<List<List<List<dynamic>>>> getChartData(String jobId, ChartDefinition chart) async =>
      await this._dataService.getChartData(jobId, chart);
  Future<List<ChartDefinition>> getJobChartDefinitions(String jobId) async =>
      await this._dataService.getJobChartDefinitions(jobId);
  Future<Success> createJobChartDefinition(String jobId, ChartDefinition chartDefinition) async =>
      await this._dataService.createJobChartDefinition(jobId, chartDefinition);
  Future<List<SocnetoComponent>> getAvailableAcquirers() async =>
      await this._dataService.getAvailableAcquirers();
  Future<List<SocnetoAnalyser>> getAvailableAnalyzers() async =>
      await this._dataService.getAvailableAnalyzers();
  Future<JobStatus> submitNewJob(String jobName, String query, List<SocnetoComponent> networks, List<SocnetoComponent> analyzers,
      String language, Map<String, Map<String, String>> credentials) async =>
      await this._dataService.submitNewJob(jobName, query, networks, analyzers, language, credentials);
  Future<JobStatus> stopJob(String jobId) async =>
      await this._dataService.stopJob(jobId);

}
