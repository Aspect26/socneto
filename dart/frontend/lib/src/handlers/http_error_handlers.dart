import 'package:angular_router/angular_router.dart';
import 'package:sw_project/src/services/base/exceptions.dart';

import '../routes.dart';


abstract class HttpErrorHandler {

  bool onHttpError(HttpException httpException);

}

class UnauthorizedHttpErrorHandler implements HttpErrorHandler {

  final Router _router;

  UnauthorizedHttpErrorHandler(this._router);

  @override
  bool onHttpError(HttpException httpException) {
    if (httpException.statusCode != 401) {
      return false;
    }

    this._router.navigate(RoutePaths.notAuthorized.toUrl());
    return true;
  }

}