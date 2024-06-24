import { Component, OnInit } from '@angular/core';
import { Dialog } from '@angular/cdk/dialog';
import { CreateRoomDialogComponent } from '../components/create-room-dialog/create-room-dialog.component';
import { DefaultRoomService } from '../../default-poker-room/services/default-room.service';
@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home-main.component.html',
  imports: [],
})
export class HomeMainComponent implements OnInit {
  constructor(public roomService: DefaultRoomService, private dialog: Dialog) {}

  ngOnInit() {}
  openCreateRoomDialog() {
    const dialogRef = this.dialog.open(CreateRoomDialogComponent);
  }
}
