import { Injectable } from '@angular/core';
import { InputBase } from '../models/input-base';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DynamicFormService {
  constructor(private client: HttpClient) {}
  getInputs(formName: FormName, parameters: FormParameters) {
    return this.client
      .post<InputBase<any>[]>(
        environment.baseApiUrl + '/formConfig/' + formName,
        parameters
      )
      .pipe(tap((inputs) => inputs.sort((a, b) => a.order - b.order)));
  }
}

export type FormParameters = { [key: string]: string };

export type FormName =
  | 'create-poker-room'
  | 'add-player'
  | 'add-issue'
  | 'add-grouped-player'
  | 'add-groups'
  | 'add-grouped-issue'
  | 'update-grouped-issue';