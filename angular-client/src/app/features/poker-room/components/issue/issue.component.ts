import { Component, Input, OnInit } from '@angular/core';
import { IssueState } from '../../models/poker-room.model';
import { RoomService } from '../../services/room.service';
import { CommonModule } from '@angular/common';
import { IconTrashComponent } from '../../../../core/icons/components/icon-trash/icon-trash.component';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { IconArrowsPointingOutComponent } from '../../../../core/icons/components/icon-arrows-pointing-out/icon-arrows-pointing-out.component';
import { MatTooltip } from '@angular/material/tooltip';
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
  constructor(
    private roomService: RoomService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {}

  selectVotingIssue() {
    this.roomService.selectVotingIssue(this.roomId, this.issue.id).subscribe();
  }

  deleteIssue() {
    this.roomService.deleteIssue(this.roomId, this.issue.id).subscribe();
  }
  replaceUrlsWithLinks(text: string): SafeHtml {
    const urlPattern =
      /((http|https|ftp):\/\/[\w?=&.\/-;#~%-]+(?![\w\s?&.\/;#~%"=-]*>))/g;
    const replacement = (match: string) =>
      `<a href="${match}" class='text-primary-blue-500 hover:text-primary-blue-300 underline' target="_blank">${match}</a>`;
    const safeHtml = this.sanitizer.bypassSecurityTrustHtml(
      text.replace(urlPattern, replacement)
    );
    return safeHtml;
  }
}
