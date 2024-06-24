import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  GroupState,
  GroupedRoomResponse,
  GroupedSpectatorState,
} from '../../../models/grouped-poker-room.model';
import { IconChevronComponent } from '../../../../../core/icons/components/icon-chevron/icon-chevron.component';
import { IconPointsComponent } from '../../../../../core/icons/components/icon-points/icon-points.component';
import { GroupedPlayerComponent } from '../grouped-player/grouped-player.component';
import { ClickOutsideDirective } from '../../../../../core/derictives/click-outside.directive';
import { IconPencilComponent } from '../../../../../core/icons/components/icon-pencil/icon-pencil.component';
import { GroupedRoomService } from '../../../services/grouped-room.service';
import {
  ChangePlayerGroupRoomRequest,
  RemoveGroupRoomRequest,
  RenameGroupRoomRequest,
} from '../../../models/grouped-poker-room-request.models';

@Component({
  selector: 'app-group',
  templateUrl: './group.component.html',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    IconChevronComponent,
    IconPointsComponent,
    GroupedPlayerComponent,
    ClickOutsideDirective,
    IconPencilComponent,
  ],
})
export class GroupComponent implements OnInit {
  @Input() group!: GroupState;
  @Input() spectator!: GroupedSpectatorState;
  @Input() roomId!: string;
  isOpened = true;
  isContextMenuOpened = false;
  isHiddenMenuOpened = false;
  isNameInputOpened = false;
  changeNameForm!: FormGroup;
  constructor(private roomService: GroupedRoomService) {}
  ngOnInit() {}
  private GetRenameInputForm(): FormGroup {
    const formGroup: any = {};
    formGroup['oldName'] = new FormControl(
      this.group.name,
      Validators.required
    );
    formGroup['newName'] = new FormControl('', Validators.required);
    return new FormGroup(formGroup);
  }
  openRenameForm() {
    if (this.isNameInputOpened) {
      this.isNameInputOpened = false;
      return;
    }
    this.changeNameForm = this.GetRenameInputForm();
    this.isNameInputOpened = true;
  }
  submitRename() {
    if (this.changeNameForm.invalid) return;
    this.isNameInputOpened = false;
    const request = new RenameGroupRoomRequest(
      this.changeNameForm.controls['oldName'].value,
      this.changeNameForm.controls['newName'].value
    );
    this.roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        if (result) {
          this.changeNameForm.controls['newName'].setValue('');
        }
      });
  }
  removeGroup() {
    const request = new RemoveGroupRoomRequest(this.group.name);
    this.roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        if (result) {
          this.isContextMenuOpened = false;
          this.isHiddenMenuOpened = false;
        }
      });
  }

  changePlayerRoom() {
    const request = new ChangePlayerGroupRoomRequest(this.group.name);
    this.roomService
      .executeCommand(this.roomId, request)
      .subscribe((result) => {
        if (result) {
          this.isContextMenuOpened = false;
          this.isHiddenMenuOpened = false;
        }
      });
  }

  get votedCount(): number {
    return this.group.players.filter((x) => x.votingState.voted).length;
  }
}
