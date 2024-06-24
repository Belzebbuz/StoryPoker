import { Component, Input, OnInit } from '@angular/core';
import {
  IssueOrder,
  IssueState,
  SetIssueListOrderDefaultRoomRequest,
  SetIssueOrderDefaultRoomRequest,
} from '../../models/default-poker-room.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { IssueComponent } from '../issue/issue.component';
import { Dialog } from '@angular/cdk/dialog';
import { AddIssueDialogComponent } from '../add-issue-dialog/add-issue-dialog.component';
import { IconBarsArrowDownComponent } from '../../../../core/icons/components/icon-bars-arrow-down/icon-bars-arrow-down.component';
import { IconBarsArrowUpComponent } from '../../../../core/icons/components/icon-bars-arrow-up/icon-bars-arrow-up.component';
import { DefaultRoomService } from '../../services/default-room.service';
import {
  CdkDragDrop,
  CdkDropList,
  CdkDrag,
  moveItemInArray,
} from '@angular/cdk/drag-drop';
@Component({
  selector: 'app-issues-list',
  standalone: true,
  templateUrl: './issues-list.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    IssueComponent,
    AddIssueDialogComponent,
    IconBarsArrowDownComponent,
    IconBarsArrowUpComponent,
    CdkDropList,
    CdkDrag,
  ],
})
export class IssuesListComponent implements OnInit {
  @Input() issues?: IssueState[];
  @Input() roomId!: string;
  @Input() spectator = false;
  @Input() issueOrder!: IssueOrder;
  constructor(
    private dialog: Dialog,
    private roomService: DefaultRoomService
  ) {}

  ngOnInit() {}

  addIssueDialog() {
    this.dialog.open(AddIssueDialogComponent, {
      data: {
        roomId: this.roomId,
      },
    });
  }
  drop(event: CdkDragDrop<IssueState[]>) {
    const oldIssueOrder = this.issues![event.currentIndex].order;
    const issue = this.issues![event.previousIndex].id;
    moveItemInArray(this.issues!, event.previousIndex, event.currentIndex);
    const command = new SetIssueOrderDefaultRoomRequest(issue, oldIssueOrder);
    this.roomService.executeCommand(this.roomId, command).subscribe();
  }
  setIssueOrder(orderBy: IssueOrder) {
    const request = new SetIssueListOrderDefaultRoomRequest(orderBy);
    this.roomService.executeCommand(this.roomId, request).subscribe();
  }
}
