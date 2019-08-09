import 'dart:convert';

import 'package:http/http.dart';
import 'package:sw_project/src/services/base/http_service_base.dart';

abstract class HttpServiceBasicAuthBase extends HttpServiceBase {

  var _base64Credentials;

  HttpServiceBasicAuthBase(String api_url, String api_prefix) : super(api_url, api_prefix);

  void setCredentials(String username, String password) {
    this._base64Credentials = base64Encode(utf8.encode("$username:$password"));
  }

  void setToken(String authToken) {
    this._base64Credentials = authToken;
  }

  void unsetCredentials() {
    this._base64Credentials = null;
  }

  String getAuthToken() {
    return this._base64Credentials;
  }

  @override
  Future<Response> httpGet(String path, { Map<String, String> headers }) async {
    if (this._base64Credentials != null) {
      headers = this._appendAuthorizationHeader(headers);
    }

    return await super.httpGet(path, headers: headers);
  }

  @override
  Future<Response> httpPost(String path, Map data, {Map<String, String> headers}) async {
    if (this._base64Credentials != null) {
      headers = this._appendAuthorizationHeader(headers);
    }

    return await super.httpPost(path, data, headers: headers);
  }

  Map<String, String> _appendAuthorizationHeader(Map<String, String> headers) {
    return this.appendHeader(headers, "Authorization", "Basic $_base64Credentials");
  }

}