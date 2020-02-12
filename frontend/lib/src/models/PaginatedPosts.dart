import 'package:sw_project/src/models/AnalyzedPost.dart';
import 'package:sw_project/src/models/Paging.dart';

class PaginatedAnalyzedPosts {

  final List<AnalyzedPost> posts;
  final Paging paging;

  PaginatedAnalyzedPosts(this.posts, this.paging);

  PaginatedAnalyzedPosts.fromMap(Map data) :
      posts = (data["data"] as List<dynamic>).map((dynamic innerData) => AnalyzedPost.fromMap(innerData as Map)).toList(),
      paging = Paging.fromMap(data["pagination"]);
}
