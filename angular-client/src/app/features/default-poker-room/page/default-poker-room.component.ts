import { Dialog } from '@angular/cdk/dialog';
import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, input, model } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { NotifiactionType } from '../../../core/signalr/models/signalr.models';
import { NotificationService } from '../../../core/signalr/services/notification.service';
import { SignalrService } from '../../../core/signalr/services/signalr.service';
import { AddPlayerDialogComponent } from '../components/add-player-dialog/add-player-dialog.component';
import { IssueComponent } from '../components/issue/issue.component';
import { IssuesListComponent } from '../components/issues-list/issues-list.component';
import { PlayersListComponent } from '../components/players-list/players-list.component';
import { VotingComponent } from '../components/voting/voting.component';
import {
  GetRoomStateResponse,
  UpdateSettingsDefaultRoomRequest,
} from '../models/default-poker-room.model';
import { DefaultRoomService } from '../services/default-room.service';
import { IconCogComponent } from '../../../core/icons/components/icon-cog/icon-cog.component';
import { ClickOutsideDirective } from '../../../core/derictives/click-outside.directive';
import { MatTooltip } from '@angular/material/tooltip';

@Component({
  selector: 'app-poker-room',
  standalone: true,
  templateUrl: './default-poker-room.component.html',
  imports: [
    PlayersListComponent,
    IssuesListComponent,
    IssueComponent,
    CommonModule,
    ReactiveFormsModule,
    VotingComponent,
    IconCogComponent,
    ClickOutsideDirective,
    FormsModule,
    MatTooltip,
  ],
})
export class DefaultPokerRoomComponent implements OnInit, OnDestroy {
  roomState?: GetRoomStateResponse;
  pokerRoomId?: string;
  subscriptions = new Subscription();
  settingsOpened = false;
  spectatorCanVote = false;
  skipBorderValues = false;
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private roomService: DefaultRoomService,
    private dialog: Dialog,
    private notification: NotificationService,
    public signalr: SignalrService
  ) {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.pokerRoomId = id;
    const subscription = this.notification
      .getObservable<string>(NotifiactionType.RoomStateUpdated)
      .subscribe((id) => {
        if (id != this.pokerRoomId) return;
        this.roomService.getState(id).subscribe((state) => {
          this.roomState = state;
          this.spectatorCanVote = this.roomState.spectatorCanVote;
          this.skipBorderValues = this.roomState.skipBorderValues;
        });
      });
    this.subscriptions.add(subscription);
  }
  ngOnDestroy(): void {
    this.signalr.unsubscribeFromRoom(this.pokerRoomId!);
    this.subscriptions.unsubscribe();
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
  initRoom() {
    if (!this.pokerRoomId) return;
    const subscription = this.roomService
      .getState(this.pokerRoomId)
      .subscribe((response) => {
        if (!response) {
          this.router.navigate(['']);
          return;
        }
        if (!response.isPlayerAdded) {
          const addResult = this.dialog.open(AddPlayerDialogComponent, {
            data: {
              roomId: this.pokerRoomId,
            },
          });
          addResult.closed.subscribe((susseded) => {
            if (!susseded) this.router.navigate(['']);
          });
        }
        this.signalr.subscribeToRoom(this.pokerRoomId!);
        this.roomState = response;
        this.spectatorCanVote = this.roomState.spectatorCanVote;
        this.skipBorderValues = this.roomState.skipBorderValues;
        document.title = 'SP - ' + this.roomState.name;
      });
    this.subscriptions.add(subscription);
  }

  setRoomSettings() {
    if (!this.pokerRoomId) return;
    this.settingsOpened = false;

    const req = new UpdateSettingsDefaultRoomRequest(
      this.spectatorCanVote,
      this.skipBorderValues
    );
    this.roomService
      .executeCommand(this.pokerRoomId, req)
      .subscribe((result) => {
        if (result) this.settingsOpened = false;
      });
  }
}
