import { Injectable } from '@angular/core';
import { InputBase } from '../models/input-base';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class InputControlService {
  constructor() {}
  toFormGroup(questions: InputBase<any>[]) {
    const group: any = {};
    questions.forEach((question) => {
      group[question.key] = this.ToControl(question);
    });
    return new FormGroup(group);
  }
  ToControl(question: InputBase<any>): FormControl {
    return question.required
      ? new FormControl(question.value || '', Validators.required)
      : new FormControl(question.value || '');
  }
}
