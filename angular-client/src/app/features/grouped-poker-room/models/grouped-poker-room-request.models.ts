import {
  IssueOrder,
  VoteStateChangeCommand,
} from '../../default-poker-room/models/default-poker-room.model';

export interface IGroupedRoomRequest {
  $type: string;
}
export class AddPlayerGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'AddPlayerGroupedRoomRequest';
  public PlayerName: string;
  public GroupName: string;
  constructor(playerName: string, groupName: string) {
    this.PlayerName = playerName;
    this.GroupName = groupName;
  }
}
export class AddGroupsRoomRequest implements IGroupedRoomRequest {
  $type = 'AddGroupsRoomRequest';
  public GroupNames: string[];
  constructor(groupNames: string[]) {
    this.GroupNames = groupNames;
  }
}

export class RenameGroupRoomRequest implements IGroupedRoomRequest {
  $type = 'RenameGroupRoomRequest';
  public OldName: string;
  public NewName: string;
  constructor(oldName: string, newName: string) {
    this.OldName = oldName;
    this.NewName = newName;
  }
}
export class RemoveGroupRoomRequest implements IGroupedRoomRequest {
  $type = 'RemoveGroupRoomRequest';
  public GroupName: string;
  constructor(groupName: string) {
    this.GroupName = groupName;
  }
}
export class ChangePlayerGroupRoomRequest implements IGroupedRoomRequest {
  $type = 'ChangePlayerGroupRoomRequest';
  public GroupName: string;
  constructor(groupName: string) {
    this.GroupName = groupName;
  }
}
export class AddIssueGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'AddIssueGroupedRoomRequest';
  public Title: string;
  public GroupNames: string[];
  constructor(title: string, groupNames: string[]) {
    this.Title = title;
    this.GroupNames = groupNames;
  }
}
export class UpdateIssueGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'UpdateIssueGroupedRoomRequest';
  public IssueId: string;
  public Title: string;
  public GroupNames: string[];
  constructor(issueId: string, title: string, groupNames: string[]) {
    this.IssueId = issueId;
    this.Title = title;
    this.GroupNames = groupNames;
  }
}
export class RemoveIssueGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'RemoveIssueGroupedRoomRequest';
  public IssueId: string;
  constructor(issueId: string) {
    this.IssueId = issueId;
  }
}
export class SetVotingIssueGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'SetVotingIssueGroupedRoomRequest';
  public IssueId: string;
  constructor(issueId: string) {
    this.IssueId = issueId;
  }
}
export class ChangeVotingStageGroupedRoomRequest
  implements IGroupedRoomRequest
{
  $type = 'ChangeVotingStageGroupedRoomRequest';
  public VotingStage: VoteStateChangeCommand;
  constructor(votingStage: VoteStateChangeCommand) {
    this.VotingStage = votingStage;
  }
}
export class SetStoryPointGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'SetStoryPointGroupedRoomRequest';
  public StoryPoint: number;
  constructor(storyPoint: number) {
    this.StoryPoint = storyPoint;
  }
}
export class SetIssueOrderByGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'SetIssueOrderByGroupedRoomRequest';
  public OrderBy: IssueOrder;
  constructor(OrderBy: IssueOrder) {
    this.OrderBy = OrderBy;
  }
}
export class SetIssueOrderGroupedRoomRequest implements IGroupedRoomRequest {
  $type = 'SetIssueOrderGroupedRoomRequest';
  public IssueId: string;
  public NewOrder: number;
  constructor(issueId: string, newOrder: number) {
    this.IssueId = issueId;
    this.NewOrder = newOrder;
  }
}
