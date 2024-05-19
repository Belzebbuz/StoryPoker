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
    [key in MessageType]: BehaviorSubject<any>;
  };
  constructor() {
    this.notifications = {
      [MessageType.RoomStateUpdated]:
        new BehaviorSubject<RoomStateUpdatedMessage>({
          messageType: MessageType.RoomStateUpdated,
          value: '',
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
    const notification = this.notifications[message.messageType];
    if (notification) {
      notification.next(message);
    }
  }
}
