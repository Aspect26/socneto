class HttpException implements Exception {

  final int statusCode;
  final String description;

  HttpException(this.statusCode, this.description);

  String getMessage() => "Http error: $statusCode - $description";

}

class BadRequestException extends HttpException {
  BadRequestException() : super(400, "Bad Request");
}

class NotAuthorizedException extends HttpException {
  NotAuthorizedException() : super(401, "Not authorized");
}

class ForbiddenException extends HttpException {
  ForbiddenException() : super(403, "Forbidden");
}

class NotFoundException extends HttpException {
  NotFoundException() : super(404, "Not found");
}

class InternalServerErrorException extends HttpException {
  InternalServerErrorException() : super(500, "Internal server error");
}

class ServiceUnavailableException extends HttpException {
  ServiceUnavailableException() : super(503, "Service unavailable");
}