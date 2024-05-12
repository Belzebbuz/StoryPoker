import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputBase } from '../../models/input-base';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dynamic-input',
  standalone: true,
  templateUrl: './dynamic-input.component.html',
  imports: [CommonModule, ReactiveFormsModule],
})
export class DynamicInputComponent implements OnInit {
  @Input() question!: InputBase<string>;
  @Input() form!: FormGroup;
  @Input() submitted = false;
  get isError() {
    const formControl = this.form.controls[this.question.key];
    return formControl.errors && (formControl.touched || this.submitted);
  }
  constructor() {}

  ngOnInit() {}
}
