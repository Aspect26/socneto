class User {
  final String username;

  const User(this.username);

  User.fromMap(Map data) :
        username = data["username"] ?? "";
}