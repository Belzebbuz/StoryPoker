import { IssueOrder } from '../../default-poker-room/models/default-poker-room.model';

export interface GroupedRoomResponse {
  name: string;
  player: GroupedCurrentPlayer;
  spectator: GroupedSpectatorState;
  votingIssue?: GroupedIssueState | null;
  groups: GroupState[];
  issues: GroupedIssueState[];
  issueOrder: IssueOrder;
}

export interface GroupState {
  name: string;
  players: GroupedPlayerState[];
}

export interface GroupedIssueState {
  id: string;
  title: string;
  stage: VotingStage;
  order: number;
  groupPoints: GroupStoryPoints[];
}

export interface GroupStoryPoints {
  name: string;
  storyPoints?: number | null;
  fibonacciStoryPoints?: number | null;
}

export interface GroupedPlayerState {
  id: string;
  name: string;
  isCurrentPlayer: boolean;
  votingState: GroupedPlayerStoryPointState;
  votingStage?: VotingStage;
}

export interface GroupedPlayerStoryPointState {
  voted: boolean;
  value?: number | null;
}

export interface GroupedCurrentPlayer {
  id: string;
  isAdded: boolean;
  canVote: boolean;
}

export interface GroupedSpectatorState {
  id: string;
  name: string;
  isCurrentPlayer: boolean;
  inRoom: boolean;
}

export enum VotingStage {
  NotStarted = 0,
  Voting = 1,
  VoteEnding = 2,
  VoteEnded = 3,
}
