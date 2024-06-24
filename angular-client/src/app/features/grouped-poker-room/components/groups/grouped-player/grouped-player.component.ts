import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  GroupedPlayerState,
  VotingStage,
} from '../../../models/grouped-poker-room.model';

@Component({
  selector: 'app-grouped-player',
  templateUrl: './grouped-player.component.html',
  standalone: true,
  imports: [CommonModule],
})
export class GroupedPlayerComponent implements OnInit {
  @Input() player!: GroupedPlayerState;
  constructor() {}

  ngOnInit() {}
  getPointsState(): ShowPointsState {
    const stage = this.player.votingStage;
    if (stage == VotingStage.NotStarted || !this.player.votingStage)
      return ShowPointsState.Cross;
    if (
      (stage == VotingStage.Voting || stage == VotingStage.VoteEnding) &&
      !this.player.votingState.voted
    )
      return ShowPointsState.Voting;
    if (
      (stage == VotingStage.Voting || stage == VotingStage.VoteEnding) &&
      this.player.votingState.voted &&
      this.player.votingState.value == null
    )
      return ShowPointsState.Voted;
    if (stage == VotingStage.VoteEnded && !this.player.votingState.voted)
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
}
