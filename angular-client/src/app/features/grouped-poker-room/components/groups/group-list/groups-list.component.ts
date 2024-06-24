import { Component, Input, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GroupedRoomResponse } from '../../../models/grouped-poker-room.model';
import { GroupComponent } from '../group/group.component';
import { IconEyeComponent } from '../../../../../core/icons/components/icon-eye/icon-eye.component';
import { Dialog } from '@angular/cdk/dialog';
import { AddGroupComponent } from '../add-group/add-group.component';

@Component({
  selector: 'app-grouped-players-list',
  templateUrl: './groups-list.component.html',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    GroupComponent,
    IconEyeComponent,
  ],
})
export class GroupsListComponent implements OnInit {
  @Input() roomState!: GroupedRoomResponse;
  @Input() roomId!: string;
  constructor(private dialog: Dialog) {}

  ngOnInit() {}
  addGroups() {
    const addResult = this.dialog.open(AddGroupComponent, {
      data: {
        roomId: this.roomId,
      },
    });
  }
}
