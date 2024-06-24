import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { GroupedIssueState } from '../../../models/grouped-poker-room.model';
import { AddGroupedIssueComponent } from '../add-grouped-issue/add-grouped-issue.component';
import { Dialog } from '@angular/cdk/dialog';
import {
  CdkDrag,
  CdkDragDrop,
  CdkDragHandle,
  CdkDropList,
  moveItemInArray,
} from '@angular/cdk/drag-drop';
import { IconBarsArrowDownComponent } from '../../../../../core/icons/components/icon-bars-arrow-down/icon-bars-arrow-down.component';
import { IconBarsArrowUpComponent } from '../../../../../core/icons/components/icon-bars-arrow-up/icon-bars-arrow-up.component';
import { IssueOrder } from '../../../../default-poker-room/models/default-poker-room.model';
import { GroupedIssueComponent } from '../grouped-issue/grouped-issue.component';
import { GroupedRoomService } from '../../../services/grouped-room.service';
import {
  SetIssueOrderByGroupedRoomRequest,
  SetIssueOrderGroupedRoomRequest,
} from '../../../models/grouped-poker-room-request.models';

@Component({
  selector: 'app-grouped-issues-list',
  standalone: true,
  templateUrl: './grouped-issues-list.component.html',
  imports: [
    CommonModule,
    AddGroupedIssueComponent,
    IconBarsArrowDownComponent,
    IconBarsArrowUpComponent,
    CdkDropList,
    CdkDrag,
    CdkDragHandle,
    GroupedIssueComponent,
  ],
})
export class GroupedIssuesListComponent implements OnInit {
  @Input() issues: GroupedIssueState[] = [];
  @Input() isSpectator = false;
  @Input() roomId!: string;
  @Input() issueOrder!: IssueOrder;
  constructor(
    private dialog: Dialog,
    private roomService: GroupedRoomService
  ) {}

  ngOnInit() {}
  addIssueDialog() {
    this.dialog.open(AddGroupedIssueComponent, {
      data: {
        roomId: this.roomId,
      },
    });
  }
  drop(event: CdkDragDrop<GroupedIssueState[]>) {
    const oldIssueOrder = this.issues![event.currentIndex].order;
    const issue = this.issues![event.previousIndex].id;
    moveItemInArray(this.issues!, event.previousIndex, event.currentIndex);
    const request = new SetIssueOrderGroupedRoomRequest(issue, oldIssueOrder);
    this.roomService.executeCommand(this.roomId, request).subscribe();
  }
  setIssueOrder(orderBy: IssueOrder) {
    const request = new SetIssueOrderByGroupedRoomRequest(orderBy);
    this.roomService.executeCommand(this.roomId, request).subscribe();
  }
}
