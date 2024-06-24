import { Component, Inject, OnInit } from '@angular/core';
import { GroupedRoomService } from '../../../services/grouped-room.service';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { FormGroup } from '@angular/forms';
import { AddPlayerGroupedRoomRequest } from '../../../models/grouped-poker-room-request.models';
import { AsyncPipe } from '@angular/common';
import { DynamicFormComponent } from '../../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';

@Component({
  selector: 'app-add-grouped-player',
  templateUrl: './add-grouped-player.component.html',
  standalone: true,
  imports: [AsyncPipe, DynamicFormComponent],
})
export class AddGroupedPlayerComponent implements OnInit {
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
    const request = new AddPlayerGroupedRoomRequest(
      form.controls['playerName'].value,
      form.controls['groupName'].value
    );
    this._roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        this._dialogRef.close(result);
      });
  }
}
