import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthorizationService {
  constructor(private client: HttpClient) {}
  public login() {
    return this.client.get(environment.baseApiUrl + '/account/login');
  }
}
