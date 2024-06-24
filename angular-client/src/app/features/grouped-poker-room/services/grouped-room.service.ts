import { Injectable } from '@angular/core';
import {
  AddPlayerGroupedRoomRequest,
  IGroupedRoomRequest,
} from '../models/grouped-poker-room-request.models';
import { HttpClient } from '@angular/common/http';
import { GroupedRoomResponse } from '../models/grouped-poker-room.model';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GroupedRoomService {
  constructor(private client: HttpClient) {}
  addPlayer(roomId: string, request: AddPlayerGroupedRoomRequest) {
    return this.client.post<boolean>(
      environment.baseApiUrl + '/room/g/' + roomId + '/players',
      request
    );
  }
  getState(roomId: string) {
    return this.client.get<GroupedRoomResponse>(
      environment.baseApiUrl + '/room/g/' + roomId
    );
  }
  executeCommand(
    roomId: string,
    request: IGroupedRoomRequest
  ): Observable<boolean> {
    return this.client.post<boolean>(
      environment.baseApiUrl + '/room/g/' + roomId + '/command',
      request
    );
  }
}
