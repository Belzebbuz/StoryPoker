import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import {
  GetRoomStateResponse,
  VoteStateChangeCommand,
} from '../../models/poker-room.model';
import { RoomService } from '../../services/room.service';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-voting',
  standalone: true,
  templateUrl: './voting.component.html',
  imports: [CommonModule],
})
export class VotingComponent implements OnInit {
  @Input() roomState!: GetRoomStateResponse;
  @Input() roomId!: string;
  votePoints = [0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89];

  constructor(
    private roomService: RoomService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {}
  setPlayerStoryPoint(value: number) {
    this.roomService.setPlayerStoryPoints(this.roomId!, value).subscribe();
  }
  changeVoteState(stage: VoteStateChangeCommand) {
    this.roomService.changeVoteStage(this.roomId!, stage).subscribe();
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
