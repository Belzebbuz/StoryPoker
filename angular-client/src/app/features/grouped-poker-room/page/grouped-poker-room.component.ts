import { Component, OnDestroy, OnInit } from '@angular/core';
import { GroupedRoomService } from '../services/grouped-room.service';
import { GroupedRoomResponse } from '../models/grouped-poker-room.model';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { GroupedVotingComponent } from '../components/grouped-voting/grouped-voting.component';
import { Dialog } from '@angular/cdk/dialog';
import { NotificationService } from '../../../core/signalr/services/notification.service';
import { SignalrService } from '../../../core/signalr/services/signalr.service';
import { AddGroupedPlayerComponent } from '../components/groups/add-grouped-player/add-grouped-player.component';
import { NotifiactionType } from '../../../core/signalr/models/signalr.models';
import { AddPlayerGroupedRoomRequest } from '../models/grouped-poker-room-request.models';
import { GroupsListComponent } from '../components/groups/group-list/groups-list.component';
import { GroupedIssuesListComponent } from '../components/issues/grouped-issues-list/grouped-issues-list.component';

@Component({
  selector: 'app-grouped-poker-room',
  standalone: true,
  templateUrl: './grouped-poker-room.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    GroupedIssuesListComponent,
    GroupsListComponent,
    GroupedVotingComponent,
  ],
})
export class GroupedPokerRoomComponent implements OnInit, OnDestroy {
  roomState?: GroupedRoomResponse;
  roomId?: string;
  subscriptions = new Subscription();
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private roomService: GroupedRoomService,
    private dialog: Dialog,
    private notification: NotificationService,
    public signalr: SignalrService
  ) {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.roomId = id;
  }
  ngOnInit() {
    if (this.signalr.state$.value) {
      this.initRoom();
      return;
    }
    this.signalr.state$.subscribe((connected) => {
      if (connected) this.initRoom();
    });
  }
  ngOnDestroy(): void {
    this.signalr.unsubscribeFromRoom(this.roomId!);
    this.subscriptions.unsubscribe();
  }

  private SubscribeEvents() {
    const subscription = this.notification
      .getObservable<string>(NotifiactionType.RoomStateUpdated)
      .subscribe((id) => {
        if (id != this.roomId) return;
        this.roomService.getState(id).subscribe((state) => {
          this.roomState = state;
        });
      });
    this.subscriptions.add(subscription);
  }
  private initRoom() {
    if (!this.roomId) return;
    const subscription = this.roomService
      .getState(this.roomId)
      .subscribe((response) => {
        if (!response) {
          this.router.navigate(['']);
          return;
        }
        this.AddPlayerIfNotExist(response);
        this.AddSpectatorIfNotExist(response);
        this.signalr.subscribeToRoom(this.roomId!);
        this.SubscribeEvents();
        this.roomState = response;
        document.title = 'SP - ' + this.roomState.name;
      });
    this.subscriptions.add(subscription);
  }

  private AddPlayerIfNotExist(response: GroupedRoomResponse) {
    if (!response.player.isAdded) {
      const addResult = this.dialog.open(AddGroupedPlayerComponent, {
        data: {
          roomId: this.roomId,
        },
      });
      addResult.closed.subscribe((susseded) => {
        if (!susseded) this.router.navigate(['']);
      });
    }
  }

  private AddSpectatorIfNotExist(response: GroupedRoomResponse) {
    if (response.spectator.isCurrentPlayer && !response.spectator.inRoom)
      this.roomService
        .executeCommand(this.roomId!, new AddPlayerGroupedRoomRequest('', ''))
        .subscribe();
  }
}
