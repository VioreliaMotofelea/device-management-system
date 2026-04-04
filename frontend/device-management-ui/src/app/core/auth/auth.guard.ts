import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStorageService } from './auth-storage.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const storage = inject(AuthStorageService);
  const router = inject(Router);

  if (storage.isLoggedIn()) {
    return true;
  }

  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl: state.url },
  });
};
