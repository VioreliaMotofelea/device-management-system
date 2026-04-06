import { Component, inject, OnInit, signal } from '@angular/core';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DEVICE_TYPE_VALUES } from './device.constants';
import type { DeviceWriteDto } from './device-write.model';
import { DeviceService } from './device.service';
import { getApiErrorMessage } from '../../core/http/api-error';

@Component({
  selector: 'app-device-upsert',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './device-upsert.component.html',
  styleUrl: './device-upsert.component.css',
})
export class DeviceUpsertComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly devicesApi = inject(DeviceService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly typeOptions = DEVICE_TYPE_VALUES;
  protected readonly loading = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly submitError = signal<string | null>(null);
  protected readonly submitting = signal(false);
  protected readonly isEdit = signal(false);
  private deviceId: number | null = null;

  protected readonly form = this.fb.group({
    name: ['', [Validators.required]],
    manufacturer: ['', [Validators.required]],
    type: ['phone', [Validators.required]],
    operatingSystem: ['', [Validators.required]],
    osVersion: ['', [Validators.required]],
    processor: ['', [Validators.required]],
    ramAmount: ['', [Validators.required]],
    description: ['', [Validators.required]],
    location: ['', [Validators.required]],
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      if (!Number.isFinite(id)) {
        void this.router.navigateByUrl('/devices');
        return;
      }
      this.isEdit.set(true);
      this.deviceId = id;
      this.loadDevice(id);
    }
  }

  private loadDevice(id: number): void {
    this.loadError.set(null);
    this.loading.set(true);
    this.devicesApi.getById(id).subscribe({
      next: (d) => {
        const typeLower = d.type?.toLowerCase() ?? 'phone';
        const type =
          this.typeOptions.includes(typeLower as (typeof DEVICE_TYPE_VALUES)[number])
            ? typeLower
            : 'phone';
        this.form.patchValue({
          name: d.name,
          manufacturer: d.manufacturer,
          type,
          operatingSystem: d.operatingSystem,
          osVersion: d.osVersion,
          processor: d.processor,
          ramAmount: d.ramAmount,
          description: d.description ?? '',
          location: d.location,
        });
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.loadError.set(
          getApiErrorMessage(err, 'Could not load device.'),
        );
      },
    });
  }

  protected submit(): void {
    this.submitError.set(null);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const body: DeviceWriteDto = {
      name: v.name.trim(),
      manufacturer: v.manufacturer.trim(),
      type: v.type.trim().toLowerCase(),
      operatingSystem: v.operatingSystem.trim(),
      osVersion: v.osVersion.trim(),
      processor: v.processor.trim(),
      ramAmount: v.ramAmount.trim(),
      description: v.description.trim(),
      location: v.location.trim(),
    };

    const missing = Object.values(body).some((x) => x.length === 0);
    if (missing) {
      this.submitError.set('All fields must have a value.');
      return;
    }

    if (!this.typeOptions.includes(body.type as (typeof DEVICE_TYPE_VALUES)[number])) {
      this.submitError.set("Type must be 'phone' or 'tablet'.");
      return;
    }

    this.submitting.set(true);
    const req =
      this.deviceId === null
        ? this.devicesApi.create(body)
        : this.devicesApi.update(this.deviceId, body);

    req.subscribe({
      next: async (saved) => {
        this.submitting.set(false);
        await this.router.navigate(['/devices', saved.id]);
      },
      error: (err) => {
        this.submitting.set(false);
        this.submitError.set(
          getApiErrorMessage(err, 'Could not save device.'),
        );
      },
    });
  }
}
