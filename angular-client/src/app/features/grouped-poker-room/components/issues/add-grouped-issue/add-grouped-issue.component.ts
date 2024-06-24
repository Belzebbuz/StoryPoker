import { Component, Inject, OnInit } from '@angular/core';
import { GroupedRoomService } from '../../../services/grouped-room.service';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { DynamicFormComponent } from '../../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { AsyncPipe } from '@angular/common';
import { FormGroup } from '@angular/forms';
import { AddIssueGroupedRoomRequest } from '../../../models/grouped-poker-room-request.models';

@Component({
  selector: 'app-add-grouped-issue',
  templateUrl: './add-grouped-issue.component.html',
  standalone: true,
  imports: [AsyncPipe, DynamicFormComponent],
})
export class AddGroupedIssueComponent implements OnInit {
  roomId: string;
  constructor(
    private _roomService: GroupedRoomService,
    private _dialogRef: DialogRef,
    @Inject(DIALOG_DATA)
    public data: { roomId: string }
  ) {
    this.roomId = data.roomId;
  }

  ngOnInit() {}
  onSubmit(form: FormGroup) {
    const request = new AddIssueGroupedRoomRequest(
      form.controls['title'].value,
      form.controls['groupNames'].value
    );
    this._roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        this._dialogRef.close(result);
      });
  }
}
