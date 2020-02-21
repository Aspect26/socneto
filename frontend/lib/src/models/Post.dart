class Post {
  final String originalId;
  final String text;
  final String originalText;
  final String authorId;
  final String language;
  final DateTime postedAt;

  Post(this.originalId, this.text, this.originalText, this.authorId, this.language, this.postedAt);

  Post.fromMap(Map data) :
      originalId = data["original_id"],
      text = data["text"],
      originalText = data["original_text"],
      authorId = data["author_id"],
      language = data["language"],
      postedAt = data["posted_at"] != null? DateTime.parse(data["posted_at"]) : null;
}