import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  GroupedRoomResponse,
  VotingStage,
} from '../../models/grouped-poker-room.model';
import { LinkWrapperService } from '../../../../core/services/link-wrapper.service';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subscription } from 'rxjs';
import { VoteStateChangeCommand } from '../../../default-poker-room/models/default-poker-room.model';
import { IconCogComponent } from '../../../../core/icons/components/icon-cog/icon-cog.component';
import { GroupedRoomService } from '../../services/grouped-room.service';
import {
  ChangeVotingStageGroupedRoomRequest,
  SetStoryPointGroupedRoomRequest,
} from '../../models/grouped-poker-room-request.models';
import { NotificationService } from '../../../../core/signalr/services/notification.service';
import { NotifiactionType } from '../../../../core/signalr/models/signalr.models';

@Component({
  selector: 'app-grouped-voting',
  templateUrl: './grouped-voting.component.html',
  standalone: true,
  imports: [CommonModule, MatTooltipModule, IconCogComponent],
})
export class GroupedVotingComponent implements OnInit, OnDestroy {
  @Input() roomState!: GroupedRoomResponse;
  @Input() roomId!: string;
  @Input() isSpectator = false;
  public timer?: number;
  votePoints = [0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89];
  subscriptions = new Subscription();
  constructor(
    public linkWrapperService: LinkWrapperService,
    private roomService: GroupedRoomService,
    private notification: NotificationService
  ) {}
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  ngOnInit() {
    const sub = this.notification
      .getObservable<number>(NotifiactionType.RoomTimerChanged)
      .subscribe((value) => {
        if (value == 0) this.timer = undefined;
        else this.timer = value;
      });
    this.subscriptions.add(sub);
  }
  setPlayerStoryPoint(value: number) {
    let request = new SetStoryPointGroupedRoomRequest(value);
    this.roomService.executeCommand(this.roomId!, request).subscribe();
  }
  changeVoteState(stage: VoteStateChangeCommand) {
    let request = new ChangeVotingStageGroupedRoomRequest(stage);
    this.roomService.executeCommand(this.roomId!, request).subscribe();
  }
  showDeck(): boolean {
    if (!this.roomState.votingIssue) return false;
    return (
      !this.isSpectator &&
      this.roomState.votingIssue.stage != VotingStage.NotStarted &&
      this.roomState.player.canVote
    );
  }
}
