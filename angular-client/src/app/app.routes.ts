import { Routes } from '@angular/router';
import { HomeMainComponent } from './features/home/page/home-main.component';
import { DefaultPokerRoomComponent } from './features/default-poker-room/page/default-poker-room.component';
import { GroupedPokerRoomComponent } from './features/grouped-poker-room/page/grouped-poker-room.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeMainComponent,
  },
  {
    path: 'room/d/:id',
    component: DefaultPokerRoomComponent,
  },
  {
    path: 'room/g/:id',
    component: GroupedPokerRoomComponent,
  },
];
