import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthStorageService } from '../../core/auth/auth-storage.service';
import { getApiErrorMessage } from '../../core/http/api-error';
import { ConfirmDialogComponent } from '../../shared/ui/confirm-dialog/confirm-dialog.component';
import { DeviceService } from './device.service';
import type { Device } from './device.types';

@Component({
  selector: 'app-device-list',
  imports: [RouterLink, ConfirmDialogComponent],
  templateUrl: './device-list.component.html',
  styleUrl: './device-list.component.css',
})
export class DeviceListComponent implements OnInit {
  private readonly devicesApi = inject(DeviceService);
  protected readonly authStorage = inject(AuthStorageService);

  protected readonly devices = signal<Device[]>([]);
  protected readonly loading = signal(true);
  protected readonly listError = signal<string | null>(null);
  protected readonly actionError = signal<string | null>(null);
  protected readonly busyId = signal<number | null>(null);
  protected readonly deleteTarget = signal<Device | null>(null);

  protected readonly currentUserId = computed(
    () => this.authStorage.session()?.user.id ?? null,
  );

  protected readonly deleteDialogMessage = computed(() => {
    const d = this.deleteTarget();
    return d
      ? `Delete “${d.name}” (${d.manufacturer})? This cannot be undone.`
      : '';
  });

  protected readonly deleteInProgress = computed(() => {
    const t = this.deleteTarget();
    const b = this.busyId();
    return t !== null && b === t.id;
  });

  ngOnInit(): void {
    this.refresh();
  }

  protected refresh(): void {
    this.listError.set(null);
    this.loading.set(true);
    this.devicesApi.getAll().subscribe({
      next: (list) => {
        this.devices.set(list);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.listError.set(
          getApiErrorMessage(err, 'Could not load devices.'),
        );
      },
    });
  }

  protected assign(id: number): void {
    this.runRowAction(id, this.devicesApi.assign(id));
  }

  protected unassign(id: number): void {
    this.runRowAction(id, this.devicesApi.unassign(id));
  }

  protected openDeleteDialog(device: Device): void {
    this.deleteTarget.set(device);
  }

  protected closeDeleteDialog(): void {
    if (this.deleteInProgress()) {
      return;
    }
    this.deleteTarget.set(null);
  }

  protected confirmDelete(): void {
    const device = this.deleteTarget();
    if (!device) {
      return;
    }

    this.actionError.set(null);
    this.busyId.set(device.id);
    this.devicesApi.delete(device.id).subscribe({
      next: () => {
        this.devices.update((list) => list.filter((d) => d.id !== device.id));
        this.busyId.set(null);
        this.deleteTarget.set(null);
      },
      error: (err) => {
        this.busyId.set(null);
        this.deleteTarget.set(null);
        this.actionError.set(
          getApiErrorMessage(err, 'Could not delete device.'),
        );
      },
    });
  }

  private runRowAction(id: number, action$: Observable<Device>): void {
    this.actionError.set(null);
    this.busyId.set(id);
    action$.subscribe({
      next: (updated) => {
        this.devices.update((list) =>
          list.map((d) => (d.id === updated.id ? updated : d)),
        );
        this.busyId.set(null);
      },
      error: (err) => {
        this.busyId.set(null);
        this.actionError.set(
          getApiErrorMessage(err, 'Action could not be completed.'),
        );
      },
    });
  }

  protected canAssign(device: Device): boolean {
    return !device.assignedUserId;
  }

  protected canUnassign(device: Device): boolean {
    const uid = this.currentUserId();
    if (uid === null || !device.assignedUserId) {
      return false;
    }
    return device.assignedUserId === uid;
  }

  protected rowBusy(id: number): boolean {
    return this.busyId() === id;
  }
}
