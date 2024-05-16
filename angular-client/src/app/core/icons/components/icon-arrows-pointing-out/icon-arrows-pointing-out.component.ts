import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-arrows-pointing-out',
  standalone: true,
  templateUrl: './icon-arrows-pointing-out.component.html',
  imports: [CommonModule],
})
export class IconArrowsPointingOutComponent implements OnInit {
  @Input() className: string = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
