import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-trash',
  standalone: true,
  templateUrl: './icon-trash.component.html',
  imports: [CommonModule],
})
export class IconTrashComponent implements OnInit {
  @Input() className: string = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
