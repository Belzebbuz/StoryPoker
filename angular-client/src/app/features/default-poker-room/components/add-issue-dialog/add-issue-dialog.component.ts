import { AsyncPipe } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { DynamicFormComponent } from '../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { DefaultRoomService } from '../../services/default-room.service';
import {
  AddIssueDefaultRoomRequest,
  AddIssueRequest,
} from '../../models/default-poker-room.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-add-issue-dialog',
  standalone: true,
  templateUrl: './add-issue-dialog.component.html',
  imports: [AsyncPipe, DynamicFormComponent],
})
export class AddIssueDialogComponent implements OnInit {
  roomId: string;
  constructor(
    private _roomService: DefaultRoomService,
    private _dialogRef: DialogRef,
    @Inject(DIALOG_DATA) public data: { roomId: string }
  ) {
    this.roomId = data.roomId;
  }

  ngOnInit() {}
  onSubmit(form: FormGroup) {
    const request = new AddIssueDefaultRoomRequest(
      form.controls['title'].value
    );
    this._roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        this._dialogRef.close(result);
      });
  }
}
