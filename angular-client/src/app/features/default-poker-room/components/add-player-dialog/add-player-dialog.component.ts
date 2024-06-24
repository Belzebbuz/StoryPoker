import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { DynamicFormComponent } from '../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { DefaultRoomService } from '../../services/default-room.service';
import { AddPlayerDefaultRoomRequest } from '../../models/default-poker-room.model';

@Component({
  selector: 'app-add-player-dialog',
  standalone: true,
  templateUrl: './add-player-dialog.component.html',
  imports: [AsyncPipe, DynamicFormComponent],
})
export class AddPlayerDialogComponent implements OnInit {
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
    const request = new AddPlayerDefaultRoomRequest(
      form.controls['playerName'].value
    );
    this._roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        this._dialogRef.close(result);
      });
  }
}
