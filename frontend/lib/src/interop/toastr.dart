@JS()
library toastr_interop;

import 'package:js/js.dart';
import 'package:js/js_util.dart';

typedef ToastrNotificationFn = Function(String message, [String title, dynamic options]);

@JS()
class ToastrInterface {
  @JS() external ToastrNotificationFn get info;
  @JS() external ToastrNotificationFn get success;
  @JS() external ToastrNotificationFn get error;
  @JS() external ToastrNotificationFn get warning;
  @JS() external Function get remove;
  @JS() external Function get clear;
}

@JS()
external ToastrInterface get toastr;

class Toastr {
  static void success(String title, String message) => toastr.success(message, title, defaultOptions);
  static void error(String title, String message) => toastr.error(message, title, defaultOptions);
  static void info(String title, String message) => toastr.info(message, title, defaultOptions);
  static void warning(String title, String message) => toastr.warning(message, title, defaultOptions);
}

dynamic defaultOptions = toastrOptions({'positionClass': 'toast-bottom-center', 'closeButton': true, "progressBar": true});

Object toastrOptions(Map<String, dynamic> dartMap) {
  var jsObject = newObject();

  dartMap.forEach((name, value) {
    setProperty(jsObject, name, value);
  });

  return jsObject;
}
