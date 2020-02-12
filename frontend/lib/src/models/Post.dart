class Post {
  final String authorId;
  final String text;
  final DateTime postedAt;

  Post(this.authorId, this.text, this.postedAt);

  Post.fromMap(Map data) :
      authorId = data["author_id"],
      text = data["text"],
      postedAt = data["posted_at"] != null? DateTime.parse(data["posted_at"]) : null;
}