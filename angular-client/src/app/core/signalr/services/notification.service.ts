import { Injectable } from '@angular/core';
import {
  INotificationMessage,
  ISignalrNotification,
  NotifiactionType as MessageType,
  RoomStateUpdatedMessage,
} from '../models/signalr.models';
import { BehaviorSubject, Observable, filter, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notifications: {
    [key in MessageType]: BehaviorSubject<ISignalrNotification>;
  };
  constructor() {
    this.notifications = {
      [MessageType.RoomStateUpdated]: new BehaviorSubject({
        messageType: MessageType.RoomStateUpdated,
      }),
    };
  }

  public getObservable<T>(messageType: MessageType): Observable<T> {
    return this.notifications[messageType].pipe(
      filter((message) => {
        const typedMessage = message as INotificationMessage<T>;
        return typedMessage != undefined && typedMessage.value != undefined;
      }),
      map((message) => {
        const typedMessage = message as INotificationMessage<T>;
        return typedMessage.value;
      })
    );
  }
  public push(message: ISignalrNotification) {
    if (message.messageType == MessageType.RoomStateUpdated) {
      this.notifications[message.messageType].next(
        message as RoomStateUpdatedMessage
      );
    }
  }
}
