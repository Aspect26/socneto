import 'dart:async';

import 'package:sw_project/src/mock_data/tasks.dart';
import 'package:sw_project/src/models/Task.dart';

class TaskService {
  Future<List<Task>> getTasks() async => mockTasks;
}