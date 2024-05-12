import { Injectable } from '@angular/core';
import { ISnackarData } from '../models/snackbar-data.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackbarComponent } from '../components/snackbar/snackbar.component';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  constructor(private _snackBar: MatSnackBar) {}
  Show(data: ISnackarData) {
    this._snackBar.openFromComponent(SnackbarComponent, {
      data: data,
      duration: 3000,
      verticalPosition: 'top',
      horizontalPosition: 'right',
      panelClass: [`snackbar-type-fill-${data.type}`, `snackbar`],
    });
  }
}
