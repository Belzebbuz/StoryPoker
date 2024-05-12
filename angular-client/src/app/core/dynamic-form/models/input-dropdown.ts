import { InputBase } from './input-base';

export class DropdownInput extends InputBase<string> {
  override controlType = 'dropdown';
}
