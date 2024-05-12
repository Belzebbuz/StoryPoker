import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputBase } from '../../models/input-base';
import { CommonModule } from '@angular/common';
import { InputControlService } from '../../services/input-control.service';
import { DynamicInputComponent } from '../dynamic-input/dynamic-input.component';

@Component({
  selector: 'app-dynamic-form',
  standalone: true,
  templateUrl: './dynamic-form.component.html',
  imports: [CommonModule, ReactiveFormsModule, DynamicInputComponent],
})
export class DynamicFormComponent implements OnInit {
  @Input() inputs: InputBase<string>[] | null = [];
  @Input() confirmText: string = 'Создать';
  @Output() onSubmit = new EventEmitter();
  form!: FormGroup;
  payLoad = '';
  submitted = false;
  constructor(private qcs: InputControlService) {}

  ngOnInit() {
    this.form = this.qcs.toFormGroup(this.inputs as InputBase<string>[]);
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) return;
    this.onSubmit.emit(this.form);
  }
}
