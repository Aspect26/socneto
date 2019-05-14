class Post {
  final String text;
  final double sentiment;
  final List<String> keywords;

  Post(this.text, this.sentiment, this.keywords);

  Post.fromMap(Map data) :
    text = data["text"],
    sentiment = data["sentiment"],
    keywords = data["keywords"];
}