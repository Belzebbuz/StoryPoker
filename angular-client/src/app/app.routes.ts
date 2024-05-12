import { Routes } from '@angular/router';
import { HomeMainComponent } from './features/home/pages/main/home-main.component';
import { PokerRoomComponent } from './features/poker-room/page/poker-room/poker-room.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeMainComponent,
  },
  {
    path: 'room/:id',
    component: PokerRoomComponent,
  },
];
