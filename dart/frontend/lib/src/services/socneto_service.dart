import 'package:sw_project/src/handlers/http_error_handlers.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';
import 'package:sw_project/src/models/User.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_data_service.dart';
import 'package:sw_project/src/services/socneto_job_management_service.dart';


class SocnetoService {

  final _dataService = SocnetoDataService();
  final _job_management_service = SocnetoJobManagementService();

  Future<User> login(String username, String password, { List<HttpErrorHandler> onError }) async {
    var result = await this._call(this._dataService.login, [username, password], null, onError);

    if (result != null) {
      this._dataService.setCredentials(username, password);
      this._job_management_service.setCredentials(username, password);
    }

    return result;
  }

  Future<Job> getJob(String jobId, { List<HttpErrorHandler> onError }) async =>
    await this._call(this._dataService.getJob, [jobId], null, onError);

  Future<List<Job>> getUserJobs(int userId, { List<HttpErrorHandler> onError }) async =>
    await this._call(this._dataService.getUserJobs, [userId], [], onError);

  Future<List<Post>> getJobPosts(String jobId) async =>
      this._dataService.getJobPosts(jobId);

  Future<List<SocnetoComponent>> getAvailableNetworks() async =>
      this._job_management_service.getAvailableNetworks();
  Future<List<SocnetoComponent>> getAvailableAnalyzers() async =>
      this._job_management_service.getAvailableAnalyzers();
  Future<String> submitNewJob(String query, List<SocnetoComponent> networks, List<SocnetoComponent> analyzers) async =>
      this._job_management_service.submitNewJob(query, networks, analyzers);

  // TODO: can't we do this somehow nicer? :(
  Future _call(Function apiFunction, List params, errorValue, List<HttpErrorHandler> onError) async {
    try {
      if (params.isEmpty) {
        return await apiFunction();
      } else if (params.length == 1) {
        return await apiFunction(params[0]);
      } else if (params.length == 2) {
        return await apiFunction(params[0], params[1]);
      } else if (params.length == 3) {
        return await apiFunction(params[0], params[1]);
      } else {
        throw Exception("Can't use SocnetoService._call() with more the 3 params :(");
      }
    } on HttpException catch (e) {
      this._handleHttpException(e, onError);
      return errorValue;
    }
  }

  void _handleHttpException(HttpException exception, List<HttpErrorHandler> handlers) {
    if (handlers == null || handlers.isEmpty) {
      throw exception;
    }

    for (var handler in handlers) {
      if (handler.onHttpError(exception)) {
        return;
      }
    }

    throw exception;
  }

}
