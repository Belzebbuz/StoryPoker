import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputBase } from '../../models/input-base';
import { CommonModule } from '@angular/common';
import { InputControlService } from '../../services/input-control.service';
import { MultiselectInputComponent } from './multiselect-input/multiselect-input.component';
import { DropdownInputComponent } from './dropbox-input/dropdown-input.component';

@Component({
  selector: 'app-dynamic-input',
  standalone: true,
  templateUrl: './dynamic-input.component.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MultiselectInputComponent,
    DropdownInputComponent,
  ],
})
export class DynamicInputComponent implements OnInit {
  @Input() question?: InputBase<any>;
  @Input() form?: FormGroup;
  @Input() submitted = false;
  @Output() onControlAdded = new EventEmitter<InputBase<any>>();
  @Output() onControlRemoved = new EventEmitter<InputBase<any>>();
  currentGroupName?: string;
  get isError(): boolean {
    if (this.control) {
      return (
        (this.control.errors && (this.control.touched || this.submitted)) ??
        false
      );
    }
    return false;
  }
  get control() {
    if (this.question && this.form) {
      return this.form.controls[this.question.key];
    }
    return;
  }
  get multiTextValues(): string[] {
    if (this.control && this.question?.controlType == 'multitext') {
      return this.control.value as string[];
    }
    return [];
  }
  constructor() {}

  ngOnInit() {}

  addItem() {
    if (this.control && this.question?.controlType == 'multitext') {
      if (this.currentGroupName) {
        const currentItems = this.control.value;
        this.control.setValue([...currentItems, this.currentGroupName]);
        this.currentGroupName = undefined;
      }
    }
  }
  removeItem(index: number) {
    if (this.control && this.question?.controlType == 'multitext') {
      const currentItems = this.control.value as string[];
      this.control.setValue(currentItems.filter((_, i) => i !== index));
    }
  }
}
