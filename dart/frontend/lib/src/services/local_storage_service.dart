import 'dart:html';

class LocalStorageService {

  final String _TOKEN_KEY = "token";

  void storeToken(String token) {
    window.localStorage[this._TOKEN_KEY] = token;
  }

  String loadToken() {
    return window.localStorage[this._TOKEN_KEY];
  }

}