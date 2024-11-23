import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  ChangeVoteStageDefaultRoomRequest,
  GetRoomStateResponse,
  SetStoryPointDefaultRoomRequest,
  VoteStateChangeCommand,
  VotingStage,
} from '../../models/default-poker-room.model';
import { DefaultRoomService } from '../../services/default-room.service';
import { MatTooltip } from '@angular/material/tooltip';
import { LinkWrapperService } from '../../../../core/services/link-wrapper.service';
import { NotificationService } from '../../../../core/signalr/services/notification.service';
import { Subscription } from 'rxjs';
import { NotifiactionType } from '../../../../core/signalr/models/signalr.models';

@Component({
  selector: 'app-voting',
  standalone: true,
  templateUrl: './voting.component.html',
  imports: [CommonModule, MatTooltip],
})
export class VotingComponent implements OnInit, OnDestroy {
  @Input() roomState!: GetRoomStateResponse;
  @Input() roomId!: string;
  public timer?: number;
  votePoints = [0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89];
  subscriptions = new Subscription();
  constructor(
    private roomService: DefaultRoomService,
    public linkWrapperService: LinkWrapperService,
    private notification: NotificationService
  ) {
    const sub = this.notification
      .getObservable<number>(NotifiactionType.RoomTimerChanged)
      .subscribe((value) => {
        if (value == 0) this.timer = undefined;
        else this.timer = value;
      });
    this.subscriptions.add(sub);
  }
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  ngOnInit() {}
  setPlayerStoryPoint(value: number) {
    const request = new SetStoryPointDefaultRoomRequest(value);
    this.roomService.executeCommand(this.roomId!, request).subscribe();
  }
  changeVoteState(stage: VoteStateChangeCommand) {
    const request = new ChangeVoteStageDefaultRoomRequest(stage);
    this.roomService.executeCommand(this.roomId!, request).subscribe();
  }

  showDeck(): boolean {
    if (!this.roomState.votingIssue) return false;
    const isVoting = this.roomState.votingIssue.stage != VotingStage.NotStarted;
    const showSpectator = this.roomState.spectatorCanVote;
    return (!this.roomState.isSpectator || showSpectator) && isVoting;
  }
}
