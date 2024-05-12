import { Component, OnInit } from '@angular/core';
import { DynamicFormService } from '../../../../core/dynamic-form/services/dynamic-form.service';
import { DynamicFormComponent } from '../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { InputBase } from '../../../../core/dynamic-form/models/input-base';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { FormGroup } from '@angular/forms';
import { InitPokerRoomRequest } from '../../../poker-room/models/poker-room.model';
import { RoomService } from '../../../poker-room/services/room.service';
import { Router, RouterModule } from '@angular/router';
import { DialogRef } from '@angular/cdk/dialog';

@Component({
  selector: 'app-create-room-dialog',
  standalone: true,
  templateUrl: './create-room-dialog.component.html',
  imports: [AsyncPipe, DynamicFormComponent, RouterModule],
})
export class CreateRoomDialogComponent implements OnInit {
  questions$: Observable<InputBase<any>[]>;
  constructor(
    formService: DynamicFormService,
    private roomService: RoomService,
    private router: Router,
    private dialogRef: DialogRef
  ) {
    this.questions$ = formService.getInputs('create-poker-room');
  }
  ngOnInit() {}

  onSubmit(form: FormGroup) {
    const request = new InitPokerRoomRequest(
      form.controls['roomName'].value,
      form.controls['playerName'].value
    );
    this.roomService.createRoom(request).subscribe((response) => {
      if (!response) {
        this.dialogRef.close();
      }
      this.router.navigate([response.link]);
      this.dialogRef.close();
    });
  }
}
