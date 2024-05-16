import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SignalrService } from './core/signalr/services/signalr.service';
import { AuthorizationService } from './core/authorization/services/authorization.service';
import { NavBarComponent } from './features/navbar/components/nav-bar/nav-bar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavBarComponent],
  templateUrl: './app.component.html',
})
export class AppComponent {
  constructor(
    private signalr: SignalrService,
    private auth: AuthorizationService
  ) {
    document.title = 'SP';
    auth.login().subscribe((completed) => {
      if (completed) {
        this.signalr.startConnection();
        this.signalr.addNotificationListener();
      }
    });
  }
}
