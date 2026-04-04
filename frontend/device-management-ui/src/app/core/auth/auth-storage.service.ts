import { isPlatformBrowser } from '@angular/common';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import type { AuthSession } from './auth.types';

const STORAGE_KEY = 'device-management.auth';

@Injectable({ providedIn: 'root' })
export class AuthStorageService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly sessionSignal = signal<AuthSession | null>(this.readSession());

  // current session; null when logged out or on the server
  readonly session = this.sessionSignal.asReadonly();

  getAccessToken(): string | null {
    const s = this.sessionSignal();
    if (!s || this.isExpired(s)) {
      return null;
    }
    return s.accessToken;
  }

  isLoggedIn(): boolean {
    const s = this.sessionSignal();
    return s !== null && !this.isExpired(s);
  }

  setSession(session: AuthSession): void {
    this.sessionSignal.set(session);
    if (isPlatformBrowser(this.platformId)) {
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
      } catch {
        // ignore quota / private mode
      }
    }
  }

  clear(): void {
    this.sessionSignal.set(null);
    if (isPlatformBrowser(this.platformId)) {
      try {
        localStorage.removeItem(STORAGE_KEY);
      } catch {
        // ignore
      }
    }
  }

  private readSession(): AuthSession | null {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) {
        return null;
      }
      const parsed = JSON.parse(raw) as AuthSession;
      if (
        !parsed?.accessToken ||
        !parsed?.expiresAtUtc ||
        !parsed?.user?.id
      ) {
        return null;
      }
      if (this.isExpired(parsed)) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return parsed;
    } catch {
      return null;
    }
  }

  private isExpired(session: AuthSession): boolean {
    const exp = Date.parse(session.expiresAtUtc);
    if (Number.isNaN(exp)) {
      return true;
    }
    return Date.now() >= exp;
  }
}
