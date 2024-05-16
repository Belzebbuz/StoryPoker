import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-icon-poker-hand',
  standalone: true,
  templateUrl: './icon-poker-hand.component.html',
  imports: [CommonModule],
})
export class IconPokerHandComponent implements OnInit {
  @Input() className = 'w-6 h-6';
  constructor() {}

  ngOnInit() {}
}
