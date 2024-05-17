import { Component, Input, OnInit } from '@angular/core';
import { IssueOrder, IssueState } from '../../models/poker-room.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { IssueComponent } from '../issue/issue.component';
import { Dialog } from '@angular/cdk/dialog';
import { AddIssueDialogComponent } from '../add-issue-dialog/add-issue-dialog.component';
import { IconBarsArrowDownComponent } from '../../../../core/icons/components/icon-bars-arrow-down/icon-bars-arrow-down.component';
import { IconBarsArrowUpComponent } from '../../../../core/icons/components/icon-bars-arrow-up/icon-bars-arrow-up.component';
import { RoomService } from '../../services/room.service';
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
  constructor(private dialog: Dialog, private roomService: RoomService) {}

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
    this.roomService
      .setIssueNewOrder(this.roomId, issue, oldIssueOrder)
      .subscribe();
  }
  setIssueOrder(orderBy: IssueOrder) {
    this.roomService.setIssuesOrder(this.roomId, orderBy).subscribe();
  }
}
