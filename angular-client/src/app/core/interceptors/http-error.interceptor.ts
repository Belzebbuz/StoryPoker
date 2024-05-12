import {
  HttpEvent,
  HttpRequest,
  HttpHandlerFn,
  HttpEventType,
  HttpErrorResponse,
  HttpResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';

import { Observable, catchError, filter, map, of, tap, throwError } from 'rxjs';
import { SnackbarService } from '../snackbar/services/snackbar.service';

export function httpErrorInterceptor(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> {
  const snackbar = inject(SnackbarService);
  return next(req).pipe(
    map((response: HttpEvent<unknown>) => {
      if (
        response instanceof HttpResponse &&
        response.status < 400 &&
        !response.body
      )
        return new HttpResponse<any>({ body: true, status: response.status });
      return response;
    }),
    catchError((error: HttpErrorResponse) => {
      if (error.status == 401 || error.status == 403 || error.status == 405)
        snackbar.Show({
          message: 'Доступ запрещен',
          type: 'error',
        });
      if (error.status == 500)
        snackbar.Show({
          message: 'Внутренняя ошибка сервера',
          type: 'error',
        });
      if (error.status == 400)
        snackbar.Show({
          message: error.error,
          type: 'error',
        });
      if (error.status == 404)
        snackbar.Show({
          message: 'Не удалось найти запрашиваемый ресурс',
          type: 'error',
        });

      return of(new HttpResponse({ body: undefined, status: error.status }));
    })
  );
}

export class OperationResult {
  constructor(public succeded: boolean, public error?: string) {}
}
