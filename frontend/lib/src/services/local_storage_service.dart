import 'dart:html';

class LocalStorageService {

  final String _TOKEN_KEY = "token";
  final String _USERNAME_KEY = "username";

  void storeToken(String username, String token) {
    window.localStorage[this._TOKEN_KEY] = token;
    window.localStorage[this._USERNAME_KEY] = username;
  }

  /// Loads username and token from the localstorage.
  /// Returns list, where first value is the username and second the Basic token
  /// Returns null, if one of the two values is not present in the localstorage
  List loadUsernameToken() {
    if (window.localStorage.containsKey(this._TOKEN_KEY)
        && window.localStorage.containsKey(this._USERNAME_KEY)) {
      return [window.localStorage[this._USERNAME_KEY],
        window.localStorage[this._TOKEN_KEY]];
    }

    return null;
  }

}