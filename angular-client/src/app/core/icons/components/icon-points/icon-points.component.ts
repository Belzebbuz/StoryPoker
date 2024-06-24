import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-points',
  templateUrl: './icon-points.component.html',
  imports: [CommonModule],
  standalone: true,
})
export class IconPointsComponent implements OnInit {
  @Input() className: string = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
