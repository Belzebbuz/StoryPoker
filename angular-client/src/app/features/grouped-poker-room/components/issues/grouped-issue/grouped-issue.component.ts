import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { GroupedIssueState } from '../../../models/grouped-poker-room.model';
import { LinkWrapperService } from '../../../../../core/services/link-wrapper.service';
import { MatTooltip } from '@angular/material/tooltip';
import { IconTrashComponent } from '../../../../../core/icons/components/icon-trash/icon-trash.component';
import { IconPointsComponent } from '../../../../../core/icons/components/icon-points/icon-points.component';
import { ClickOutsideDirective } from '../../../../../core/derictives/click-outside.directive';
import { UpdateGroupedIssueComponent } from '../update-grouped-issue/update-grouped-issue.component';
import { Dialog } from '@angular/cdk/dialog';
import { GroupedRoomService } from '../../../services/grouped-room.service';
import {
  RemoveGroupRoomRequest,
  RemoveIssueGroupedRoomRequest,
  SetVotingIssueGroupedRoomRequest,
} from '../../../models/grouped-poker-room-request.models';

@Component({
  selector: 'app-grouped-issue',
  templateUrl: './grouped-issue.component.html',
  standalone: true,
  imports: [
    CommonModule,
    IconTrashComponent,
    IconPointsComponent,
    MatTooltip,
    ClickOutsideDirective,
  ],
})
export class GroupedIssueComponent implements OnInit {
  @Input() issue!: GroupedIssueState;
  @Input() isSpectator = false;
  @Input() roomId!: string;
  isContextMenuOpened = false;
  constructor(
    public linkWrapperService: LinkWrapperService,
    private dialog: Dialog,
    private roomService: GroupedRoomService
  ) {}

  ngOnInit() {}
  openUpdateDialog() {
    this.dialog.open(UpdateGroupedIssueComponent, {
      data: {
        roomId: this.roomId,
        issueId: this.issue.id,
      },
    });
  }
  selectVotingIssue() {
    const request = new SetVotingIssueGroupedRoomRequest(this.issue.id);
    this.roomService.executeCommand(this.roomId, request).subscribe();
  }

  deleteIssue() {
    const request = new RemoveIssueGroupedRoomRequest(this.issue.id);
    this.roomService.executeCommand(this.roomId, request).subscribe();
  }
}
