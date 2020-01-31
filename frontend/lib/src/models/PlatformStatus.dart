import '../utils.dart';

class PlatformStatus {

  final SocnetoComponentStatus jmsStatus;
  final SocnetoComponentStatus storageStatus;
  final SocnetoComponentStatus backendStatus;

  PlatformStatus(this.jmsStatus, this.storageStatus, this.backendStatus);

  PlatformStatus.fromMap(Map data) :
      jmsStatus = getEnumByString(SocnetoComponentStatus.values, data["jms"], SocnetoComponentStatus.STOPPED),
      storageStatus = getEnumByString(SocnetoComponentStatus.values, data["storage"], SocnetoComponentStatus.STOPPED),
      backendStatus = SocnetoComponentStatus.RUNNING;

}

enum SocnetoComponentStatus {
  UNKNOWN,
  STOPPED,
  STARTING,
  RUNNING
}