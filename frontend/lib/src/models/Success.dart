class Success {
  final bool success;

  Success(this.success);

  Success.fromMap(Map data) :
      success = data['success'] ?? false;
}