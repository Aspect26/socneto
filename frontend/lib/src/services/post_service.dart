import 'dart:async';

import 'package:sw_project/src/mock_data/posts.dart';
import 'package:sw_project/src/models/Post.dart';

class PostService {
  Future<List<Post>> getPosts(int taskId) async => mockPosts;
}