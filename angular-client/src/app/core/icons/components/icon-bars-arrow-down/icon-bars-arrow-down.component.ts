import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-bars-arrow-down',
  standalone: true,
  templateUrl: './icon-bars-arrow-down.component.html',
  imports: [CommonModule],
})
export class IconBarsArrowDownComponent implements OnInit {
  @Input() className: string = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
