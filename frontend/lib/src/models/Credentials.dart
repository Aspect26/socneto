class TwitterCredentials {

  String apiKey;
  String apiSecretKey;
  String accessToken;
  String accessTokenSecret;

  Map toJson() => {
    "api_key": this.apiKey,
    "api_secret_key": this.apiSecretKey,
    "access_token": this.accessToken,
    "access_token_secret": this.accessTokenSecret
  };

}


class RedditCredentials {

  String appId;
  String appSecret;
  String refreshToken;

  Map toJson() => {
    "app_id": this.appId,
    "app_secret": this.appSecret,
    "refresh_token": this.refreshToken
  };

}