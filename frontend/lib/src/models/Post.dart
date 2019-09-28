class Post {
  final String authorId;
  final String text;
  final DateTime postedAt;

  Post(this.authorId, this.text, this.postedAt);

  Post.fromMap(Map data) :
      authorId = data["authorId"],
      text = data["text"],
      postedAt = data["postedAt"] != null? DateTime.parse(data["postedAt"]) : null;
}