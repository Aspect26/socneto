import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart';
import 'package:http/http.dart' as http;
import 'package:meta/meta.dart';
import 'package:sw_project/src/services/base/exceptions.dart';

class HttpServiceBase {

  final String _api_url;
  final String _api_prefix;

  HttpServiceBase(this._api_url, this._api_prefix);

  Future<T> get<T>(String path, Function(Map) mapJson) async {
    var response = await this.httpGet(path);
    this._checkResponseStatusCode(response);
    return this._extractJson<T>(response, mapJson);
  }

  Future<T> post<T>(String path, Map<String, dynamic> body, Function(Map) mapJson) async {
    var response = await this.httpPost(path, body);
    this._checkResponseStatusCode(response);
    return this._extractJson<T>(response, mapJson);
  }

  Future<List<T>> getList<T>(String path, Function mapJson) async {
    var response = await this.httpGet(path);
    this._checkResponseStatusCode(response);
    return this._extractJsonList<T>(response, mapJson);
  }

  Future<List<T>> postList<T>(String path, Map<String, dynamic> body, Function mapJson) async {
    var response = await this.httpPost(path, body);
    this._checkResponseStatusCode(response);
    return this._extractJsonList<T>(response, mapJson);
  }

  @protected
  Future<Response> httpGet(String path, {Map<String, String> headers}) async =>
      await http.get(this.getFullApiCallPath(path), headers: headers);

  @protected
  Future<Response> httpPost(String path, Map<String, dynamic> data, {Map<String, String> headers}) async =>
    await http.post(
        this.getFullApiCallPath(path), body: utf8.encode(json.encode(data)),
        headers: this.appendHeader(headers, "Content-Type", "application/json"));

  @protected
  Map<String, String> appendHeader(Map<String, String> headers, String headerName, String headerValue) {
    if (headers == null) {
      headers = {};
    }

    headers[headerName] = headerValue;
    return headers;
  }

  String getFullApiCallPath(String path) =>
      this._api_url + "/" + this._api_prefix + "/" + path;

  void _checkResponseStatusCode(Response response) {
    switch (response.statusCode) {
      case 200: return;
      case 400: throw BadRequestException(); break;
      case 401: throw NotAuthorizedException(); break;
      case 403: throw ForbiddenException(); break;
      case 404: throw NotFoundException(); break;
      case 500: throw InternalServerErrorException(); break;
      case 503: throw  ServiceUnavailableException(); break;
      default: throw HttpException(response.statusCode, "No description"); break;
    }
  }

  List<T> _extractJsonList<T>(Response response, Function mapJson) {
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

}
