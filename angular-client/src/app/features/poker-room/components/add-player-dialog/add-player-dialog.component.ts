import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { InputBase } from '../../../../core/dynamic-form/models/input-base';
import { AsyncPipe } from '@angular/common';
import { DynamicFormComponent } from '../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { DynamicFormService } from '../../../../core/dynamic-form/services/dynamic-form.service';
import { RoomService } from '../../services/room.service';
import { AddPlayerRequest } from '../../models/poker-room.model';

@Component({
  selector: 'app-add-player-dialog',
  standalone: true,
  templateUrl: './add-player-dialog.component.html',
  imports: [AsyncPipe, DynamicFormComponent],
})
export class AddPlayerDialogComponent implements OnInit {
  roomId: string;
  questions$: Observable<InputBase<any>[]>;
  constructor(
    formService: DynamicFormService,
    private _roomService: RoomService,
    private _dialogRef: DialogRef,
    @Inject(DIALOG_DATA) public data: { roomId: string }
  ) {
    this.roomId = data.roomId;
    this.questions$ = formService.getInputs('add-player');
  }

  ngOnInit() {}
  onSubmit(form: FormGroup) {
    const request = new AddPlayerRequest(form.controls['playerName'].value);
    this._roomService.addPlayer(this.roomId, request).subscribe((result) => {
      this._dialogRef.close(result);
    });
  }
}
