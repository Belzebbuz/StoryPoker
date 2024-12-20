import { Component, Input, OnInit } from '@angular/core';
import { IssueState, PlayerState } from '../../models/default-poker-room.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { PlayerComponent } from '../player/player.component';

@Component({
  selector: 'app-players-list',
  standalone: true,
  templateUrl: './players-list.component.html',
  imports: [CommonModule, ReactiveFormsModule, PlayerComponent],
})
export class PlayersListComponent implements OnInit {
  @Input() players?: PlayerState[];
  @Input() currentPlayerId!: string;
  @Input() votingIssue?: IssueState;
  @Input() currentPlayerIsSpectator = false;
  @Input() roomId!: string;
  @Input() specatorCanVote = false;
  constructor() {}

  ngOnInit() {}
}
