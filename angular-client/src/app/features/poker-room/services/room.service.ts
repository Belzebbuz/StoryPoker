import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  AddIssueRequest,
  AddPlayerRequest,
  GetRoomStateResponse,
  InitPokerRoomRequest,
  InitPokerRoomResponse,
  IssueOrder,
  VoteStateChangeCommand,
} from '../models/poker-room.model';

@Injectable({
  providedIn: 'root',
})
export class RoomService {
  constructor(private client: HttpClient) {}
  getState(roomId: string): Observable<GetRoomStateResponse> {
    return this.client.get<GetRoomStateResponse>(
      environment.baseApiUrl + '/room/' + roomId
    );
  }

  createRoom(request: InitPokerRoomRequest): Observable<InitPokerRoomResponse> {
    return this.client.post<InitPokerRoomResponse>(
      environment.baseApiUrl + '/room',
      request
    );
  }

  addPlayer(roomId: string, request: AddPlayerRequest): Observable<boolean> {
    return this.client.post<boolean>(
      environment.baseApiUrl + '/room/' + roomId + '/players',
      request
    );
  }

  addIssue(roomId: string, request: AddIssueRequest): Observable<boolean> {
    return this.client.post<boolean>(
      environment.baseApiUrl + '/room/' + roomId + '/issues',
      request
    );
  }

  selectVotingIssue(roomId: string, issueId: string): Observable<boolean> {
    return this.client.put<boolean>(
      environment.baseApiUrl +
        '/room/' +
        roomId +
        '/current-issue?issueId=' +
        issueId,
      {}
    );
  }
  setPlayerStoryPoints(
    roomId: string,
    storyPoint: number
  ): Observable<boolean> {
    return this.client.put<boolean>(
      environment.baseApiUrl +
        '/room/' +
        roomId +
        '/issues/current-issue/story-point?storyPoint=' +
        storyPoint,
      {}
    );
  }

  changeVoteStage(roomId: string, stage: VoteStateChangeCommand) {
    return this.client.put<boolean>(
      environment.baseApiUrl + '/room/' + roomId + '/vote-stage?stage=' + stage,
      {}
    );
  }
  deleteIssue(roomId: string, issueId: string) {
    return this.client.delete<boolean>(
      environment.baseApiUrl + '/room/' + roomId + '/issues/' + issueId
    );
  }

  setNewSpectator(roomId: string, playerId: string) {
    return this.client.put<boolean>(
      environment.baseApiUrl +
        '/room/' +
        roomId +
        '/players/spectator?playerId=' +
        playerId,
      {}
    );
  }

  setIssuesOrder(roomId: string, order: IssueOrder) {
    return this.client.put<boolean>(
      environment.baseApiUrl +
        '/room/' +
        roomId +
        '/issues/order?order=' +
        order,
      {}
    );
  }

  setIssueNewOrder(roomId: string, issueId: string, newOrder: number) {
    return this.client.put<boolean>(
      environment.baseApiUrl +
        '/room/' +
        roomId +
        '/issues/' +
        issueId +
        '/order?newOrder=' +
        newOrder,
      {}
    );
  }
}
