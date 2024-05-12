import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment';
import { ISignalrNotification } from '../models/signalr.models';
import { NotificationService } from './notification.service';
import { BehaviorSubject } from 'rxjs';
import { AuthorizationService } from '../../authorization/services/authorization.service';
@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  #hubConnection!: signalR.HubConnection;
  state$ = new BehaviorSubject<boolean>(false);
  #baseUrl = environment.baseApiUrl + '/notifications';
  #hubOptions: signalR.IHttpConnectionOptions = {
    withCredentials: true,
  };
  constructor(private notification: NotificationService) {}
  public startConnection = () => {
    this.#hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(
        environment.production
          ? signalR.LogLevel.None
          : signalR.LogLevel.Information
      )
      .withUrl(this.#baseUrl, this.#hubOptions)
      .withAutomaticReconnect()
      .build();
    this.#hubConnection
      .start()
      .then(() => {
        this.state$.next(true);
        return console.log('Connection started');
      })
      .catch();
  };
  public subscribeToRoom(roomId: string) {
    this.#hubConnection.onreconnected(() => this.subscribeToRoom(roomId));
    if (this.state$.value) {
      this.#hubConnection.send('AddPlayerToRoom', roomId);
      return;
    }
    this.state$.subscribe((connected) => {
      if (connected) this.#hubConnection.send('AddPlayerToRoom', roomId);
    });
  }
  public unsubscribeFromRoom(roomId: string) {
    if (this.state$.value) {
      this.#hubConnection.send('RemovePlayerFromRoom', roomId);
      return;
    }
    this.state$.subscribe((connected) => {
      if (connected) this.#hubConnection.send('RemovePlayerFromRoom', roomId);
    });
  }
  public disconnect() {
    this.#hubConnection?.stop();
  }
  public addNotificationListener() {
    this.#hubConnection.on(
      'NotificationServer',
      (data: ISignalrNotification) => {
        this.notification.push(data);
      }
    );
  }
}
