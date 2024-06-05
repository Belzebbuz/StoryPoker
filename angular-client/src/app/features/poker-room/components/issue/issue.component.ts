import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IssueState } from '../../models/poker-room.model';
import { RoomService } from '../../services/room.service';
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
    private roomService: RoomService,
    public linkWrapperService: LinkWrapperService
  ) {}

  ngOnInit() {}

  selectVotingIssue() {
    this.roomService.selectVotingIssue(this.roomId, this.issue.id).subscribe();
  }

  deleteIssue() {
    this.removing = true;
    this.roomService
      .deleteIssue(this.roomId, this.issue.id)
      .subscribe((_) => (this.removing = false));
  }
}
