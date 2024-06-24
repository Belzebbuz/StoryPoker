import { Component, Inject, OnInit } from '@angular/core';
import { DynamicFormComponent } from '../../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { AsyncPipe } from '@angular/common';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { GroupedRoomService } from '../../../services/grouped-room.service';
import { FormParameters } from '../../../../../core/dynamic-form/services/dynamic-form.service';
import { UpdateIssueGroupedRoomRequest } from '../../../models/grouped-poker-room-request.models';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-update-grouped-issue',
  templateUrl: './update-grouped-issue.component.html',
  standalone: true,
  imports: [AsyncPipe, DynamicFormComponent],
})
export class UpdateGroupedIssueComponent implements OnInit {
  parameters: FormParameters = {};
  constructor(
    private _roomService: GroupedRoomService,
    private _dialogRef: DialogRef,
    @Inject(DIALOG_DATA)
    public data: { roomId: string; issueId: string }
  ) {
    this.parameters['roomId'] = data.roomId;
    this.parameters['issueId'] = data.issueId;
  }
  ngOnInit() {}
  onSubmit(form: FormGroup) {
    const request = new UpdateIssueGroupedRoomRequest(
      this.parameters['issueId'],
      form.controls['title'].value,
      form.controls['groupNames'].value
    );
    this._roomService
      .executeCommand(this.parameters['roomId'], request)
      .subscribe((result) => {
        this._dialogRef.close(result);
      });
  }
}
