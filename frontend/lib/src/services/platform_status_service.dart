import 'dart:async';

import 'package:sw_project/src/models/PlatformStatus.dart';
import 'package:sw_project/src/services/socneto_service.dart';
import 'package:synchronized/synchronized.dart';
import 'package:tuple/tuple.dart';

typedef StatusChangeCallback = Function(SocnetoComponentStatusChangedEvent changeEvent, PlatformStatus currentPlatformStatus);

class PlatformStatusService {

  final Duration repeatInterval = const Duration(seconds:5);
  final SocnetoService _socnetoService;
  final Lock _lock = Lock();

  PlatformStatus _platformStatus = PlatformStatus(SocnetoComponentStatus.UNKNOWN, SocnetoComponentStatus.UNKNOWN, SocnetoComponentStatus.UNKNOWN);
  Timer _currentPollingTimer;
  List<Tuple2<Object, StatusChangeCallback>> _pollers = [];

  PlatformStatusService(this._socnetoService);

  PlatformStatus getCurrentStatus() => this._platformStatus;

  void subscribeToChanges(Object listener, StatusChangeCallback callback) {
    this._lock.synchronized(() {
      this._pollers.add(Tuple2(listener, callback));
    });

    this._pollStatus();
    if (this._currentPollingTimer == null) {
      this._currentPollingTimer = Timer.periodic(repeatInterval, (Timer t) => this._pollStatus());
    }
  }

  void unsubscribeFromChanges(Object listener) {
    this._lock.synchronized(() {
      this._pollers.removeWhere((poller) => poller.item1 == listener);
      if (this._pollers.isEmpty && this._currentPollingTimer != null) {
        this._currentPollingTimer.cancel();
        this._currentPollingTimer = null;
      }
    });
  }

  void _pollStatus() async {
    PlatformStatus newPlatformStatus;

    try {
      newPlatformStatus = await this._socnetoService.getPlatformStatus();
    } catch (e) {
      newPlatformStatus = PlatformStatus(SocnetoComponentStatus.UNKNOWN, SocnetoComponentStatus.UNKNOWN, SocnetoComponentStatus.STOPPED);
    }

    try {
      this._checkPlatformStatusChanges(newPlatformStatus);
    } finally {
      this._platformStatus = newPlatformStatus;
    }
  }

  void _checkPlatformStatusChanges(PlatformStatus newPlatformStatus) {
    if (newPlatformStatus.backendStatus != this._platformStatus.backendStatus) {
      this._distributeChangeEvent(SocnetoComponentType.BACKEND, newPlatformStatus.backendStatus, newPlatformStatus);
    }
    if (newPlatformStatus.storageStatus != this._platformStatus.storageStatus) {
      this._distributeChangeEvent(SocnetoComponentType.STORAGE, newPlatformStatus.storageStatus, newPlatformStatus);
    }
    if (newPlatformStatus.jmsStatus != this._platformStatus.jmsStatus) {
      this._distributeChangeEvent(SocnetoComponentType.JMS, newPlatformStatus.jmsStatus, newPlatformStatus);
    }
  }

  void _distributeChangeEvent(SocnetoComponentType component, SocnetoComponentStatus newStatus, PlatformStatus newPlatformStatus) {
    SocnetoComponentStatus previousBackendStatus;
    switch (component) {
      case SocnetoComponentType.BACKEND:
        previousBackendStatus = this._platformStatus.backendStatus;
        break;
      case SocnetoComponentType.STORAGE:
        previousBackendStatus = this._platformStatus.storageStatus;
        break;
      case SocnetoComponentType.JMS:
        previousBackendStatus = this._platformStatus.jmsStatus;
        break;
    }

    var changeEvent = SocnetoComponentStatusChangedEvent(component, previousBackendStatus, newStatus);

    this._lock.synchronized(() {
      this._pollers.forEach((poller) {
        poller.item2(changeEvent, newPlatformStatus);
      });
    });
  }
}

class SocnetoComponentStatusChangedEvent {
  final SocnetoComponentType component;
  final SocnetoComponentStatus previousStatus;
  final SocnetoComponentStatus newStatus;

  SocnetoComponentStatusChangedEvent(this.component, this.previousStatus, this.newStatus);
}

enum SocnetoComponentType {
  BACKEND,
  STORAGE,
  JMS
}