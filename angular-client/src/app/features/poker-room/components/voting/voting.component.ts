import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  GetRoomStateResponse,
  VoteStateChangeCommand,
  VotingStage,
} from '../../models/poker-room.model';
import { RoomService } from '../../services/room.service';
import { MatTooltip } from '@angular/material/tooltip';
import { LinkWrapperService } from '../../../../core/services/link-wrapper.service';

@Component({
  selector: 'app-voting',
  standalone: true,
  templateUrl: './voting.component.html',
  imports: [CommonModule, MatTooltip],
})
export class VotingComponent implements OnInit {
  @Input() roomState!: GetRoomStateResponse;
  @Input() roomId!: string;
  votePoints = [0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89];

  constructor(
    private roomService: RoomService,
    public linkWrapperService: LinkWrapperService
  ) {}

  ngOnInit() {}
  setPlayerStoryPoint(value: number) {
    this.roomService.setPlayerStoryPoints(this.roomId!, value).subscribe();
  }
  changeVoteState(stage: VoteStateChangeCommand) {
    this.roomService.changeVoteStage(this.roomId!, stage).subscribe();
  }

  showDeck(): boolean {
    if (!this.roomState.votingIssue) return false;
    return (
      !this.roomState.isSpectator &&
      this.roomState.votingIssue.stage != VotingStage.NotStarted
    );
  }
}
