import {
  HttpErrorResponse,
  HttpInterceptorFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthStorageService } from './auth-storage.service';

const authAnonymousPaths = /\/api\/auth\/(login|register)$/i;

// clears session and sends the user to login when the API returns 401 
// e.g.: expired token, except on auth endpoints
export const authErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const storage = inject(AuthStorageService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        !authAnonymousPaths.test(req.url)
      ) {
        storage.clear();
        void router.navigate(['/login'], {
          queryParams: { returnUrl: router.url },
        });
      }
      return throwError(() => error);
    }),
  );
};
