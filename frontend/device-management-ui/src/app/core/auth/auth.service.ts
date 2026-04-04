import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL } from '../api/api-base-url.token';
import { AuthStorageService } from './auth-storage.service';
import type { AuthResponse, LoginRequest, RegisterRequest } from './auth.types';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);
  private readonly storage = inject(AuthStorageService);
  private readonly router = inject(Router);

  login(body: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/auth/login`, body)
      .pipe(tap((res) => this.persist(res)));
  }

  register(body: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/auth/register`, body)
      .pipe(tap((res) => this.persist(res)));
  }

  logout(): void {
    this.storage.clear();
    void this.router.navigateByUrl('/login');
  }

  private persist(res: AuthResponse): void {
    this.storage.setSession({
      accessToken: res.accessToken,
      expiresAtUtc: res.expiresAtUtc,
      user: res.user,
    });
  }
}
