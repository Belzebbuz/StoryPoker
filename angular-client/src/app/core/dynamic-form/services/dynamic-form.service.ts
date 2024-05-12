import { Injectable } from '@angular/core';
import { InputBase } from '../models/input-base';
import { TextboxInput } from '../models/input-textbox';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DynamicFormService {
  constructor() {}
  getInputs(formName: 'create-poker-room' | 'add-player' | 'add-issue') {
    let questions: InputBase<string>[] = [];
    switch (formName) {
      case 'create-poker-room':
        questions = [
          new TextboxInput({
            key: 'roomName',
            label: 'Название комнаты',
            type: 'text',
            required: true,
            order: 1,
          }),
          new TextboxInput({
            key: 'playerName',
            label: 'Имя игрока',
            type: 'text',
            required: true,
            order: 2,
          }),
        ];
        break;
      case 'add-player':
        {
          questions = [
            new TextboxInput({
              key: 'playerName',
              label: 'Имя игрока',
              type: 'text',
              required: true,
              order: 1,
            }),
          ];
        }
        break;
      case 'add-issue':
        {
          questions = [
            new TextboxInput({
              key: 'title',
              label: 'Заголовок',
              type: 'text',
              required: true,
              order: 1,
            }),
          ];
        }
        break;
      default:
        break;
    }

    return of(questions.sort((a, b) => a.order - b.order));
  }
}
