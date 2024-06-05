import { Injectable } from '@angular/core';
import {
  INotificationMessage,
  ISignalrNotification,
  NotifiactionType,
} from '../models/signalr.models';
import { BehaviorSubject, Observable, filter, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notifications: {
    [key in NotifiactionType]: BehaviorSubject<any>;
  };
  constructor() {
    this.notifications = {
      [NotifiactionType.RoomStateUpdated]:
        new BehaviorSubject<ISignalrNotification>({
          messageType: NotifiactionType.RoomStateUpdated,
        }),
      [NotifiactionType.RoomTimerChanged]:
        new BehaviorSubject<ISignalrNotification>({
          messageType: NotifiactionType.RoomTimerChanged,
        }),
    };
  }

  public getObservable<T>(messageType: NotifiactionType): Observable<T> {
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
