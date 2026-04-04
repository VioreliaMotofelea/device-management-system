import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStorageService } from './auth-storage.service';

// redirect authenticated users away from login/register
export const guestGuard: CanActivateFn = () => {
  const storage = inject(AuthStorageService);
  const router = inject(Router);

  if (storage.isLoggedIn()) {
    return router.createUrlTree(['/devices']);
  }

  return true;
};
