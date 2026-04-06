import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthStorageService } from '../../core/auth/auth-storage.service';
import { getApiErrorMessage } from '../../core/http/api-error';
import { DeviceService } from './device.service';
import type { Device } from './device.types';

@Component({
  selector: 'app-device-detail',
  imports: [RouterLink],
  templateUrl: './device-detail.component.html',
  styleUrl: './device-detail.component.css',
})
export class DeviceDetailComponent implements OnInit {
  private readonly devicesApi = inject(DeviceService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly authStorage = inject(AuthStorageService);

  protected readonly device = signal<Device | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly actionError = signal<string | null>(null);
  protected readonly busyAssign = signal(false);

  protected readonly currentUserId = computed(
    () => this.authStorage.session()?.user.id ?? null,
  );

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : NaN;
    if (!Number.isFinite(id)) {
      void this.router.navigateByUrl('/devices');
      return;
    }
    this.load(id);
  }

  private load(id: number): void {
    this.error.set(null);
    this.loading.set(true);
    this.devicesApi.getById(id).subscribe({
      next: (d) => {
        this.device.set(d);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(getApiErrorMessage(err, 'Could not load device.'));
      },
    });
  }

  protected canAssign(d: Device): boolean {
    return !d.assignedUserId;
  }

  protected canUnassign(d: Device): boolean {
    const uid = this.currentUserId();
    if (uid === null || !d.assignedUserId) {
      return false;
    }
    return d.assignedUserId === uid;
  }

  protected assign(): void {
    const d = this.device();
    if (!d) {
      return;
    }
    this.runAssign(this.devicesApi.assign(d.id));
  }

  protected unassign(): void {
    const d = this.device();
    if (!d) {
      return;
    }
    this.runAssign(this.devicesApi.unassign(d.id));
  }

  private runAssign(req$: Observable<Device>): void {
    this.actionError.set(null);
    this.busyAssign.set(true);
    req$.subscribe({
      next: (updated) => {
        this.device.set(updated);
        this.busyAssign.set(false);
      },
      error: (err) => {
        this.busyAssign.set(false);
        this.actionError.set(
          getApiErrorMessage(err, 'Could not update assignment.'),
        );
      },
    });
  }
}
