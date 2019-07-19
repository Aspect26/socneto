import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart';
import 'package:http/http.dart' as http;
import 'package:meta/meta.dart';

class HttpServiceBase {

  final String _api_url;
  final String _api_prefix;

  HttpServiceBase(this._api_url, this._api_prefix);

  Future<T> get<T>(String path, Function(Map) mapJson) async {
    var response = await this.httpGet(path);
    return this._extractJson<T>(response, mapJson);
  }

  Future<T> post<T>(String path, Map data, Function(Map) mapJson) async {
    var response = await this.httpPost(path, data);
    return this._extractJson<T>(response, mapJson);
  }

  Future<List<T>> getList<T>(String path, Function(Map) mapJson) async {
    var response = await this.httpGet(path);
    return this._extractJsonList<T>(response, mapJson);
  }

  @protected
  Future<Response> httpGet(String path, {Map<String, String> headers}) async =>
      await http.get(this._getFullApiCallPath(path), headers: headers);

  @protected
  Future<Response> httpPost(String path, Map data, {Map<String, String> headers}) async =>
    await http.post(
        this._getFullApiCallPath(path), body: utf8.encode(json.encode(data)),
        headers: this.appendHeader(headers, "Content-Type", "application/json"));

  @protected
  Map<String, String> appendHeader(Map<String, String> headers, String headerName, String headerValue) {
    if (headers == null) {
      headers = {};
    }

    headers[headerName] = headerValue;
    return headers;
  }

  List<T> _extractJsonList<T>(Response response, Function(Map) mapJson) {
    var data = (jsonDecode(response.body) as List);
    List<T> result = [];

    for (var datum in data) {
      result.add(mapJson(datum));
    }

    return result;
  }

  T _extractJson<T>(Response response, Function(Map) mapJson) {
    var data = (jsonDecode(response.body) as Map);
    return mapJson(data);
  }

  String _getFullApiCallPath(String path) =>
      this._api_url + "/" + this._api_prefix + "/" + path;

}
