import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { API_BASE_URL } from './core/api/api-base-url.token';
import { authErrorInterceptor } from './core/auth/auth-error.interceptor';
import { authInterceptor } from './core/auth/auth.interceptor';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl },
    provideHttpClient(
      withInterceptors([authInterceptor, authErrorInterceptor]),
    ),
  ],
};
