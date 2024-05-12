import { Component, Inject, OnInit, inject } from '@angular/core';
import {
  MAT_SNACK_BAR_DATA,
  MatSnackBarRef,
} from '@angular/material/snack-bar';
import { ISnackarData } from '../../models/snackbar-data.model';
@Component({
  selector: 'app-snackbar',
  standalone: true,
  templateUrl: './snackbar.component.html',
})
export class SnackbarComponent implements OnInit {
  snackBarRef = inject(MatSnackBarRef);
  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: ISnackarData) {}

  ngOnInit() {}
}
