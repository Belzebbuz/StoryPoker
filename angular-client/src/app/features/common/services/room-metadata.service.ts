import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import {
  InitPokerRoomRequest,
  InitPokerRoomResponse,
} from '../models/room-metadata.model';

@Injectable({
  providedIn: 'root',
})
export class RoomMetadataService {
  constructor(private client: HttpClient) {}
  createRoom(request: InitPokerRoomRequest): Observable<InitPokerRoomResponse> {
    return this.client.post<InitPokerRoomResponse>(
      environment.baseApiUrl + '/room/metadata',
      request
    );
  }
}
