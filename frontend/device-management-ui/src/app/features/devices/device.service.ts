import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../core/api/api-base-url.token';
import type { Device } from './device.types';

@Injectable({ providedIn: 'root' })
export class DeviceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  getAll(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices`);
  }

  assign(deviceId: number): Observable<Device> {
    return this.http.post<Device>(`${this.baseUrl}/devices/${deviceId}/assign`, {});
  }

  unassign(deviceId: number): Observable<Device> {
    return this.http.post<Device>(`${this.baseUrl}/devices/${deviceId}/unassign`, {});
  }
}
