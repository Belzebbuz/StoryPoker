export enum NotifiactionType {
  RoomStateUpdated = 0,
}

export interface ISignalrNotification {
  messageType: NotifiactionType;
}

export interface INotificationMessage<T> extends ISignalrNotification {
  value: T;
}

export interface RoomStateUpdatedMessage extends INotificationMessage<string> {}
