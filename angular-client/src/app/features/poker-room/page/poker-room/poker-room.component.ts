import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RoomService } from '../../services/room.service';
import {
  GetRoomStateResponse,
  IssueState,
  VoteStateChangeCommand,
} from '../../models/poker-room.model';
import { PlayersListComponent } from '../../components/players-list/players-list.component';
import { IssuesListComponent } from '../../components/issues-list/issues-list.component';
import { Dialog } from '@angular/cdk/dialog';
import { AddPlayerDialogComponent } from '../../components/add-player-dialog/add-player-dialog.component';
import { NotificationService } from '../../../../core/signalr/services/notification.service';
import { NotifiactionType } from '../../../../core/signalr/models/signalr.models';
import { SignalrService } from '../../../../core/signalr/services/signalr.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { IssueComponent } from '../../components/issue/issue.component';
import { Subscription } from 'rxjs';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-poker-room',
  standalone: true,
  templateUrl: './poker-room.component.html',
  imports: [
    PlayersListComponent,
    IssuesListComponent,
    IssueComponent,
    CommonModule,
    ReactiveFormsModule,
  ],
})
export class PokerRoomComponent implements OnInit, OnDestroy {
  roomState?: GetRoomStateResponse;
  pokerRoomId?: string;
  votePoints = [0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89];
  subscriptions = new Subscription();
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private roomService: RoomService,
    private dialog: Dialog,
    private notification: NotificationService,
    public signalr: SignalrService,
    private sanitizer: DomSanitizer
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
      });
    this.subscriptions.add(subscription);
  }
  setPlayerStoryPoint(value: number) {
    this.roomService.setPlayerStoryPoints(this.pokerRoomId!, value).subscribe();
  }
  changeVoteState(stage: VoteStateChangeCommand) {
    this.roomService.changeVoteStage(this.pokerRoomId!, stage).subscribe();
  }
  replaceUrlsWithLinks(text: string): SafeHtml {
    const urlPattern =
      /((http|https|ftp):\/\/[\w?=&.\/-;#~%-]+(?![\w\s?&.\/;#~%"=-]*>))/g;
    const replacement = (match: string) =>
      `<a href="${match}" class='text-primary-blue-500 hover:text-primary-blue-300 underline' target="_blank">${match}</a>`;
    const safeHtml = this.sanitizer.bypassSecurityTrustHtml(
      text.replace(urlPattern, replacement)
    );
    return safeHtml;
  }
}
