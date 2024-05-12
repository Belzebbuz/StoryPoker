import { AsyncPipe } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { DynamicFormComponent } from '../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { Observable } from 'rxjs';
import { InputBase } from '../../../../core/dynamic-form/models/input-base';
import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { DynamicFormService } from '../../../../core/dynamic-form/services/dynamic-form.service';
import { RoomService } from '../../services/room.service';
import { AddIssueRequest } from '../../models/poker-room.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-add-issue-dialog',
  standalone: true,
  templateUrl: './add-issue-dialog.component.html',
  imports: [AsyncPipe, DynamicFormComponent],
})
export class AddIssueDialogComponent implements OnInit {
  roomId: string;
  questions$: Observable<InputBase<any>[]>;
  constructor(
    formService: DynamicFormService,
    private _roomService: RoomService,
    private _dialogRef: DialogRef,
    @Inject(DIALOG_DATA) public data: { roomId: string }
  ) {
    this.roomId = data.roomId;
    this.questions$ = formService.getInputs('add-issue');
  }

  ngOnInit() {}
  onSubmit(form: FormGroup) {
    const request = new AddIssueRequest(form.controls['title'].value);
    this._roomService.addIssue(this.roomId, request).subscribe((result) => {
      this._dialogRef.close(result);
    });
  }
}
