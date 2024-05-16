export class InitPokerRoomRequest {
  constructor(public roomName: string, public playerName: string) {}
}
export interface InitPokerRoomResponse {
  link: string;
}
export class AddPlayerRequest {
  constructor(public playerName: string) {}
}

export class AddIssueRequest {
  constructor(public title: string) {}
}

export enum VoteStateChangeCommand {
  Start = 0,
  Stop = 1,
}

export enum IssueOrder {
  Asc = 0,
  Desc = 1,
}

export interface GetRoomStateResponse {
  playerId: string;
  isPlayerAdded: boolean;
  name: string;
  votingIssue?: IssueState;
  isSpectator: boolean;
  players: PlayerState[];
  issues: IssueState[];
  issueOrder: IssueOrder;
}

export interface PlayerState {
  id: string;
  name: string;
  isSpectator: boolean;
  currentVotingPoint: PlayerIssueStoryPoint;
}

export interface PlayerIssueStoryPoint {
  voted: boolean;
  value?: number;
}

export interface IssueState {
  id: string;
  title: string;
  stage: VotingStage;
  storyPoints?: number;
  order: number;
}

export enum VotingStage {
  NotStarted = 0,
  Voting = 1,
  VoteEnded = 2,
}
