import { Component, OnInit } from '@angular/core';
import { DynamicFormComponent } from '../../../../core/dynamic-form/components/dynamic-form/dynamic-form.component';
import { AsyncPipe } from '@angular/common';
import { FormGroup } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { DialogRef } from '@angular/cdk/dialog';
import { RoomMetadataService } from '../../../common/services/room-metadata.service';
import { InitPokerRoomRequest } from '../../../common/models/room-metadata.model';
import { AuthorizationService } from '../../../../core/authorization/services/authorization.service';
import { FormParameters } from '../../../../core/dynamic-form/services/dynamic-form.service';

@Component({
  selector: 'app-create-room-dialog',
  standalone: true,
  templateUrl: './create-room-dialog.component.html',
  imports: [AsyncPipe, DynamicFormComponent, RouterModule],
})
export class CreateRoomDialogComponent implements OnInit {
  constructor(
    private roomService: RoomMetadataService,
    private router: Router,
    private dialogRef: DialogRef
  ) {}
  ngOnInit() {}

  onSubmit(form: FormGroup) {
    const request = new InitPokerRoomRequest(
      form.controls['roomName'].value,
      form.controls['playerName'].value,
      form.controls['roomType'].value,
      form.controls['groupNames'] ? form.controls['groupNames'].value : []
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
