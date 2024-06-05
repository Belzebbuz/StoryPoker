export enum NotifiactionType {
  RoomStateUpdated = 0,
  RoomTimerChanged = 1,
}

export interface ISignalrNotification {
  messageType: NotifiactionType;
}

export interface INotificationMessage<T> extends ISignalrNotification {
  value: T;
}

export interface RoomStateUpdatedMessage extends INotificationMessage<string> {}
export interface RoomVoteEndingTimerMessage
  extends INotificationMessage<number> {}