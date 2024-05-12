import { Component, Input, OnInit } from '@angular/core';
import { IssueState } from '../../models/poker-room.model';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { IssueComponent } from '../issue/issue.component';
import { Dialog } from '@angular/cdk/dialog';
import { AddIssueDialogComponent } from '../add-issue-dialog/add-issue-dialog.component';

@Component({
  selector: 'app-issues-list',
  standalone: true,
  templateUrl: './issues-list.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    IssueComponent,
    AddIssueDialogComponent,
  ],
})
export class IssuesListComponent implements OnInit {
  @Input() issues?: IssueState[];
  @Input() roomId!: string;
  @Input() spectator = false;
  constructor(private dialog: Dialog) {}

  ngOnInit() {}

  addIssueDialog() {
    this.dialog.open(AddIssueDialogComponent, {
      data: {
        roomId: this.roomId,
      },
    });
  }
}
