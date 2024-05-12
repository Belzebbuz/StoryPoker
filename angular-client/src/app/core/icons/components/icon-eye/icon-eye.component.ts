import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-eye',
  standalone: true,
  templateUrl: './icon-eye.component.html',
  imports: [CommonModule],
})
export class IconEyeComponent implements OnInit {
  @Input() className: string = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
