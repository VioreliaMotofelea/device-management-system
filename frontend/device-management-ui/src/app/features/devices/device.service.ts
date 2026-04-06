import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../core/api/api-base-url.token';
import type { DeviceWriteDto } from './device-write.model';
import type { Device } from './device.types';

@Injectable({ providedIn: 'root' })
export class DeviceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  getAll(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.baseUrl}/devices`);
  }

  getById(id: number): Observable<Device> {
    return this.http.get<Device>(`${this.baseUrl}/devices/${id}`);
  }

  create(body: DeviceWriteDto): Observable<Device> {
    return this.http.post<Device>(`${this.baseUrl}/devices`, body);
  }

  update(id: number, body: DeviceWriteDto): Observable<Device> {
    return this.http.put<Device>(`${this.baseUrl}/devices/${id}`, body);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/devices/${id}`);
  }

  assign(deviceId: number): Observable<Device> {
    return this.http.post<Device>(`${this.baseUrl}/devices/${deviceId}/assign`, {});
  }

  unassign(deviceId: number): Observable<Device> {
    return this.http.post<Device>(`${this.baseUrl}/devices/${deviceId}/unassign`, {});
  }
}
