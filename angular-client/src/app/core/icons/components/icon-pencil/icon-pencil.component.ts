import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-pencil',
  templateUrl: './icon-pencil.component.html',
  standalone: true,
  imports: [CommonModule],
})
export class IconPencilComponent implements OnInit {
  @Input() className: string = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
