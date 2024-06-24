import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  IssueState,
  RemoveIssueDefaultRoomRequest,
  SetCurrentIssueDefaultRoomRequest,
} from '../../models/default-poker-room.model';
import { DefaultRoomService } from '../../services/default-room.service';
import { CommonModule } from '@angular/common';
import { IconTrashComponent } from '../../../../core/icons/components/icon-trash/icon-trash.component';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { IconArrowsPointingOutComponent } from '../../../../core/icons/components/icon-arrows-pointing-out/icon-arrows-pointing-out.component';
import { MatTooltip } from '@angular/material/tooltip';
import { LinkWrapperService } from '../../../../core/services/link-wrapper.service';
@Component({
  selector: 'app-issue',
  standalone: true,
  templateUrl: './issue.component.html',
  imports: [
    CommonModule,
    IconTrashComponent,
    CdkDragHandle,
    IconArrowsPointingOutComponent,
    MatTooltip,
  ],
})
export class IssueComponent implements OnInit {
  @Input() issue!: IssueState;
  @Input() roomId!: string;
  @Input() spectator = false;
  removing = false;
  constructor(
    private roomService: DefaultRoomService,
    public linkWrapperService: LinkWrapperService
  ) {}

  ngOnInit() {}

  selectVotingIssue() {
    const request = new SetCurrentIssueDefaultRoomRequest(this.issue.id);
    this.roomService.executeCommand(this.roomId, request).subscribe();
  }

  deleteIssue() {
    this.removing = true;
    const request = new RemoveIssueDefaultRoomRequest(this.issue.id);
    this.roomService
      .executeCommand(this.roomId, request)
      .subscribe((_) => (this.removing = false));
  }
}
