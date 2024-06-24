import { AsyncPipe } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { FormGroup } from '@angular/forms';
import { DynamicFormComponent } from '../../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { AddGroupsRoomRequest } from '../../../models/grouped-poker-room-request.models';
import { GroupedRoomService } from '../../../services/grouped-room.service';

@Component({
  selector: 'app-add-group',
  templateUrl: './add-group.component.html',
  standalone: true,
  imports: [AsyncPipe, DynamicFormComponent],
})
export class AddGroupComponent implements OnInit {
  roomId: string;
  constructor(
    private _roomService: GroupedRoomService,
    private _dialogRef: DialogRef,
    @Inject(DIALOG_DATA) public data: { roomId: string }
  ) {
    this.roomId = data.roomId;
  }
  ngOnInit() {}
  onSubmit(form: FormGroup) {
    const request = new AddGroupsRoomRequest(form.controls['groupNames'].value);
    this._roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        this._dialogRef.close(result);
      });
  }
}
