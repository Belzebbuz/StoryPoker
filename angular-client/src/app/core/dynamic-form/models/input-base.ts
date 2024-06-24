export interface InputBase<T> {
  value: T | undefined;
  key: string;
  label: string;
  required: boolean;
  order: number;
  controlType: string;
  type: string;
  options: { [key: string]: OptionValue };
}

export interface OptionValue {
  value: string;
  inputs: InputBase<any>[];
}
