import '../utils.dart';

class PlatformStatus {

  final SocnetoComponentStatus jmsStatus;
  final SocnetoComponentStatus storageStatus;

  PlatformStatus(this.jmsStatus, this.storageStatus);

  PlatformStatus.fromMap(Map data) :
      jmsStatus = getEnumByString(SocnetoComponentStatus.values, data["jms"], SocnetoComponentStatus.STOPPED),
      storageStatus = getEnumByString(SocnetoComponentStatus.values, data["storage"], SocnetoComponentStatus.STOPPED);

}

enum SocnetoComponentStatus {
  STOPPED,
  STARTING,
  RUNNING
}