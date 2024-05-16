import { Component, OnInit } from '@angular/core';
import { IconPokerHandComponent } from '../../../../core/icons/components/icon-poker-hand/icon-poker-hand.component';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  templateUrl: './nav-bar.component.html',
  imports: [IconPokerHandComponent],
})
export class NavBarComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
