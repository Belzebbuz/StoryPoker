import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-bars-arrow-up',
  standalone: true,
  templateUrl: './icon-bars-arrow-up.component.html',
  imports: [CommonModule],
})
export class IconBarsArrowUpComponent implements OnInit {
  @Input() className: string = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
