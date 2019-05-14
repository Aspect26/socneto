import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart';
import 'package:http/http.dart' as http;
import 'package:sw_project/src/models/Post.dart';
import 'package:sw_project/src/models/Task.dart';

class SocnetoService {

  static const String API_URL = "http://acheron.ms.mff.cuni.cz:39103";
  static const String API_PREFIX = "api";

  Future<List<Task>> getUserJobs(int userId) async {
    return await this.getList("user-job-statuses/$userId", (n) => Task.fromMap(n));
  }

  Future<List<Post>> getJobResult(int jobId) async {
    throw new Exception("NOT IMPLEMENTED");
  }


  Future<List<T>> getList<T>(String path, Function(Map) mapJson) async {
    var response = await http.get(API_URL + "/" + API_PREFIX + "/" + path);
    return this._extractJsonList<T>(response, mapJson);
  }

  List<T> _extractJsonList<T>(Response response, Function(Map) mapJson) {
    var data = (jsonDecode(response.body) as List);
    List<T> result = [];

    for (var datum in data) {
      result.add(mapJson(datum));
    }

    return result;
  }
}