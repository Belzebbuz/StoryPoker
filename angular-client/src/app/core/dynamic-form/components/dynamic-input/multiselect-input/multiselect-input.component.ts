import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputBase } from '../../../models/input-base';
import { CommonModule } from '@angular/common';
import { IconArrowsPointingOutComponent } from '../../../../icons/components/icon-arrows-pointing-out/icon-arrows-pointing-out.component';
import { IconChevronComponent } from '../../../../icons/components/icon-chevron/icon-chevron.component';

@Component({
  selector: 'app-multiselect-input',
  templateUrl: './multiselect-input.component.html',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    IconChevronComponent,
  ],
})
export class MultiselectInputComponent implements OnInit {
  @Input() form!: FormGroup;
  @Input() question!: InputBase<any>;
  constructor() {}
  get control() {
    if (this.question && this.form) {
      return this.form.controls[this.question.key];
    }
    return;
  }

  get selectedValues(): string[] {
    if (this.control) {
      return this.control.value as string[];
    }
    return [];
  }
  ngOnInit() {}
  addItem(value: string) {
    if (!this.control) return;
    const currentItems = this.control.value;
    this.control.setValue([...currentItems, value]);
  }
  removeItem(index: number) {
    if (this.control) {
      const currentItems = this.control.value as string[];
      this.control.setValue(currentItems.filter((_, i) => i !== index));
    }
  }
  optionDisbaled(value: string): boolean {
    if (!this.control) return true;
    const addedItems = this.control.value as string[];
    if (!addedItems) return false;
    const index = addedItems.findIndex((x) => x == value);
    return index > -1;
  }
}
