import {
  AfterContentInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputBase } from '../../models/input-base';
import { CommonModule } from '@angular/common';
import { InputControlService } from '../../services/input-control.service';
import { DynamicInputComponent } from '../dynamic-input/dynamic-input.component';
import {
  DynamicFormService,
  FormName,
  FormParameters,
} from '../../services/dynamic-form.service';

@Component({
  selector: 'app-dynamic-form',
  standalone: true,
  templateUrl: './dynamic-form.component.html',
  imports: [CommonModule, ReactiveFormsModule, DynamicInputComponent],
})
export class DynamicFormComponent implements OnInit {
  @Input() formName!: FormName;
  @Input() confirmText: string = 'Создать';
  @Input() parameters: FormParameters = {};
  @Output()
  onSubmit = new EventEmitter();
  inputs?: InputBase<any>[];
  form?: FormGroup;
  payLoad = '';
  submitted = false;
  constructor(
    private qcs: InputControlService,
    private formService: DynamicFormService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.formService
      .getInputs(this.formName, this.parameters)
      .subscribe((inputs) => {
        this.inputs = inputs;
        this.form = this.qcs.toFormGroup(this.inputs as InputBase<any>[]);
        this.cdr.detectChanges();
      });
  }

  submit() {
    if (!this.form) return;
    this.submitted = true;
    if (this.form.invalid) return;
    this.onSubmit.emit(this.form);
  }
  addInput(input: InputBase<any>) {
    this.inputs?.push(input);
  }
  removeInput(input: InputBase<any>) {
    const index = this.inputs?.findIndex((x) => x.key == input.key);
    if (!index) return;
    this.inputs?.splice(index, 1);
  }
}
