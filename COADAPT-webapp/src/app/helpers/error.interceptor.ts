import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthenticationService } from '../services';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private authenticationService: AuthenticationService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError((err) => {
        if (err.status === 401) {
          this.authenticationService.logout();
          location.reload();
        }

        if (err.status === 403)
          this.router.navigate(['/403']);

        if (err.status === 500)
          this.router.navigate(['/500']);

        let error: any;
        if (typeof err.error === 'object' && err.error !== null)
          error = err.error.title;
        else
          error = err.error;
        return throwError(error);
      })
    );
  }

}
