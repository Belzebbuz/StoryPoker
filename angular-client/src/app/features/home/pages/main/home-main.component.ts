import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { RoomService } from '../../../poker-room/services/room.service';
import { CreateRoomDialogComponent } from '../../components/create-room-dialog/create-room-dialog.component';
@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home-main.component.html',
  imports: [],
})
export class HomeMainComponent implements OnInit {
  constructor(public roomService: RoomService, private dialog: Dialog) {}

  ngOnInit() {}
  openCreateRoomDialog() {
    const dialogRef = this.dialog.open(CreateRoomDialogComponent);
  }
}
