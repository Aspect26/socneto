class Post {
  final String originalId;
  final String text;
  final String originalText;
  final DateTime postedAt;

  Post(this.originalId, this.text, this.originalText, this.postedAt);

  Post.fromMap(Map data) :
      originalId = data["original_id"],
      text = data["text"],
      originalText = data["original_text"],
      postedAt = data["posted_at"] != null? DateTime.parse(data["posted_at"]) : null;
}