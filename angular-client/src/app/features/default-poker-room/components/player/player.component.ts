import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTooltip } from '@angular/material/tooltip';
import {
  IssueState,
  PlayerState,
  SelectSpectatorDefaultRoomRequest,
  VotingStage,
} from '../../models/default-poker-room.model';
import { IconEyeComponent } from '../../../../core/icons/components/icon-eye/icon-eye.component';
import { DefaultRoomService } from '../../services/default-room.service';

@Component({
  selector: 'app-player',
  standalone: true,
  templateUrl: './player.component.html',
  imports: [CommonModule, ReactiveFormsModule, IconEyeComponent, MatTooltip],
})
export class PlayerComponent implements OnInit {
  @Input() player!: PlayerState;
  @Input() isCurrentPlayer!: boolean;
  @Input() votingIssue?: IssueState;
  @Input() currentPlayerIsSpectator = false;
  @Input() roomId!: string;
  constructor(private roomService: DefaultRoomService) {}

  ngOnInit() {}

  setNewSpectator() {
    this.roomService
      .executeCommand(
        this.roomId,
        new SelectSpectatorDefaultRoomRequest(this.player.id)
      )
      .subscribe();
  }

  showChangeSpectatorButton(): boolean {
    if (this.votingIssue)
      return (
        this.votingIssue.stage != VotingStage.Voting &&
        this.currentPlayerIsSpectator &&
        !this.player.isSpectator
      );
    return this.currentPlayerIsSpectator && !this.player.isSpectator;
  }

  getPointsState(): ShowPointsState {
    if (this.player.isSpectator) return ShowPointsState.Spectator;
    if (!this.votingIssue || this.votingIssue.stage == VotingStage.NotStarted)
      return ShowPointsState.Cross;
    if (
      (this.votingIssue.stage == VotingStage.Voting ||
        this.votingIssue.stage == VotingStage.VoteEnding) &&
      !this.player.currentVotingPoint.voted
    )
      return ShowPointsState.Voting;
    if (
      (this.votingIssue.stage == VotingStage.Voting ||
        this.votingIssue.stage == VotingStage.VoteEnding) &&
      this.player.currentVotingPoint.voted &&
      this.player.currentVotingPoint.value == null
    )
      return ShowPointsState.Voted;
    if (
      this.votingIssue.stage == VotingStage.VoteEnded &&
      !this.player.currentVotingPoint.voted
    )
      return ShowPointsState.RedCross;

    return ShowPointsState.Value;
  }
}

export enum ShowPointsState {
  Cross = 0,
  Voting = 1,
  Voted = 2,
  Value = 3,
  RedCross = 4,
  Spectator = 5,
}
