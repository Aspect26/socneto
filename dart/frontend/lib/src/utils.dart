int _SECONDS_IN_MINUTE = 60;
int _SECONDS_IN_HOUR = 60 * 60;
int _SECONDS_IN_DAY = 24 * 60 * 60;
int _SECONDS_IN_MONTH = 30 * 24 * 60 * 60;


String getDurationString(Duration duration) {
  var seconds = duration.inSeconds;

  int months = seconds ~/ _SECONDS_IN_MONTH;
  seconds -= months * _SECONDS_IN_MONTH;

  int days = seconds  ~/ _SECONDS_IN_DAY;
  seconds -= days * _SECONDS_IN_DAY;

  int hours = seconds ~/ _SECONDS_IN_HOUR;
  seconds -= hours * _SECONDS_IN_HOUR;

  int minutes = seconds ~/ _SECONDS_IN_MINUTE;
  seconds -= minutes * _SECONDS_IN_MINUTE;

  if (months != 0) {
    return "$months ${_plural(months, "month", "months")}, ${days.abs()} ${_plural(days, "day", "days")}";
  } else if (days != 0) {
    return "$days ${_plural(days, "day", "days")}, ${hours.abs()} ${_plural(hours, "hour", "hours")}";
  } else if (hours != 0) {
    return "$hours ${_plural(hours, "hour", "hours")}, ${minutes.abs()} ${_plural(minutes, "minute", "minutes")}";
  } else if (minutes != 0) {
    return "$minutes ${_plural(minutes, "minute", "minutes")}, ${seconds.abs()} ${_plural(seconds, "second", "seconds")}";
  } else {
    return "$seconds ${_plural(seconds, "second", "seconds")}";
  }
}

String _plural(int count, String singular, String plural) {
  return count.abs() != 1? plural : singular;
}

T getEnumByString<T>(List<T> allEnumValues, String value, T defaultValue) {
  return allEnumValues.firstWhere((t) => t.toString() == "${T.toString()}.$value", orElse: () => null)  ?? defaultValue;
}