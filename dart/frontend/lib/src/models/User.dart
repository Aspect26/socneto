class User {
  final String username;
  final int id;

  const User(this.username, this.id);

  User.fromMap(Map data) :
        username = data["username"] ?? "",
        id = data["id"];
}