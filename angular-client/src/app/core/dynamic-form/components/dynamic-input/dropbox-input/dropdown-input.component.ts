import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputBase } from '../../../models/input-base';
import { InputControlService } from '../../../services/input-control.service';

@Component({
  selector: 'app-dropdown-input',
  templateUrl: './dropdown-input.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
})
export class DropdownInputComponent implements OnInit {
  @Input() form!: FormGroup;
  @Input() question!: InputBase<any>;
  @Output() onControlAdded = new EventEmitter<InputBase<any>>();
  @Output() onControlRemoved = new EventEmitter<InputBase<any>>();
  selectedOption?: string;

  constructor(private inputService: InputControlService) {}

  ngOnInit() {
    if (this.question.options) {
      const optionsArray = Object.keys(this.question.options);
      const formControl = this.form.controls[this.question.key];
      formControl.patchValue(optionsArray[0]);
      this.selectedOption = optionsArray[0];
      this.AddOptionsToForm(this.selectedOption);
    }
  }
  private AddOptionsToForm(optionKey: string) {
    if (this.question) {
      this.question.options[optionKey].inputs.forEach((inp) => {
        const control = this.inputService.ToControl(inp);
        this.form?.addControl(inp.key, control);
        this.onControlAdded.emit(inp);
      });
    }
  }
  private RemoveOptionsFromForm(optionKey: string) {
    if (this.question) {
      this.question.options[optionKey].inputs.forEach((inp) => {
        this.form?.removeControl(inp.key);
        this.onControlRemoved.emit(inp);
      });
    }
  }
  addControls($event: any) {
    if (this.selectedOption) this.RemoveOptionsFromForm(this.selectedOption);
    this.AddOptionsToForm($event.target.value);
    this.selectedOption = $event.target.value;
    console.log(this.form);
  }
}
