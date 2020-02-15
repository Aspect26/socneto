import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/Paging.dart';
import 'package:sw_project/src/models/Post.dart';

class PaginatedPosts {

  final List<Post> posts;
  final Paging paging;

  PaginatedPosts(this.posts, this.paging);

  PaginatedPosts.fromMap(Map data) :
      posts = (data["data"] as List<dynamic>).map((dynamic innerData) => Post.fromMap(innerData as Map)).toList(),
      paging = Paging.fromMap(data["pagination"]);
}
