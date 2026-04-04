import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthStorageService } from './auth-storage.service';

const authAnonymousPaths = /\/api\/auth\/(login|register)$/i;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (authAnonymousPaths.test(req.url)) {
    return next(req);
  }

  const storage = inject(AuthStorageService);
  const token = storage.getAccessToken();
  if (!token) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    }),
  );
};
