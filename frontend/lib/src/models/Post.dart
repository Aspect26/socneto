class Post {
  final String text;
  final double sentiment;
  final List<String> keywords;
  final DateTime postedAt;

  Post(this.text, this.sentiment, this.keywords, this.postedAt);

  Post.fromMap(Map data) :
    text = data["text"],
    sentiment = data["sentiment"],
    keywords = (data["keywords"] as List<dynamic>).map((d) => d.toString()).toList(),
    postedAt = data["dateTime"] != null? DateTime.parse(data["dateTime"]) : null;
}