export interface IDefaultRoomRequest {
  $type: string;
}

export class AddPlayerDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'AddPlayerDefaultRoomRequest';
  public PlayerName: string;
  constructor(playerName: string) {
    this.PlayerName = playerName;
  }
}
export class SelectSpectatorDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'SelectSpectatorDefaultRoomRequest';
  public PlayerId: string;
  constructor(playerId: string) {
    this.PlayerId = playerId;
  }
}
export class ChangeVoteStageDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'ChangeVoteStageDefaultRoomRequest';
  public Stage: VoteStateChangeCommand;
  constructor(stage: VoteStateChangeCommand) {
    this.Stage = stage;
  }
}
export class AddIssueDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'AddIssueDefaultRoomRequest';
  public Title: string;
  constructor(title: string) {
    this.Title = title;
  }
}
export class SetIssueListOrderDefaultRoomRequest
  implements IDefaultRoomRequest
{
  public $type = 'SetIssueListOrderDefaultRoomRequest';
  public Order: IssueOrder;
  constructor(order: IssueOrder) {
    this.Order = order;
  }
}
export class SetIssueOrderDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'SetIssueOrderDefaultRoomRequest';
  public IssueId: string;
  public NewOrder: number;
  constructor(issueId: string, newOrder: number) {
    this.IssueId = issueId;
    this.NewOrder = newOrder;
  }
}
export class RemoveIssueDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'RemoveIssueDefaultRoomRequest';
  public IssueId: string;
  constructor(issueId: string) {
    this.IssueId = issueId;
  }
}
export class SetCurrentIssueDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'SetCurrentIssueDefaultRoomRequest';
  public IssueId: string;
  constructor(issueId: string) {
    this.IssueId = issueId;
  }
}
export class SetStoryPointDefaultRoomRequest implements IDefaultRoomRequest {
  public $type = 'SetStoryPointDefaultRoomRequest';
  public StoryPoint: number;
  constructor(storyPoint: number) {
    this.StoryPoint = storyPoint;
  }
}
export class AddIssueRequest {
  constructor(public title: string) {}
}

export enum IssueOrder {
  Asc = 0,
  Desc = 1,
}
export enum VoteStateChangeCommand {
  Start = 0,
  StartEndTimer = 1,
  Stop = 2,
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
  fibonacciStoryPoints?: number;
  order: number;
}

export enum VotingStage {
  NotStarted = 0,
  Voting = 1,
  VoteEnding = 2,
  VoteEnded = 3,
}
