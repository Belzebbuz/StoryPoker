import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  AddIssueRequest,
  AddPlayerDefaultRoomRequest,
  GetRoomStateResponse,
  IDefaultRoomRequest,
  IssueOrder,
  VoteStateChangeCommand,
} from '../models/default-poker-room.model';

@Injectable({
  providedIn: 'root',
})
export class DefaultRoomService {
  constructor(private client: HttpClient) {}

  getState(roomId: string): Observable<GetRoomStateResponse> {
    return this.client.get<GetRoomStateResponse>(
      environment.baseApiUrl + '/room/d/' + roomId
    );
  }

  executeCommand(
    roomId: string,
    request: IDefaultRoomRequest
  ): Observable<boolean> {
    return this.client.post<boolean>(
      environment.baseApiUrl + '/room/d/' + roomId + '/command',
      request
    );
  }
}
