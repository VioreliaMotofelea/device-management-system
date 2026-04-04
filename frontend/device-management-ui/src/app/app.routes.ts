import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { guestGuard } from './core/auth/guest.guard';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { DeviceDetailComponent } from './features/devices/device-detail.component';
import { DeviceListComponent } from './features/devices/device-list.component';
import { DeviceUpsertComponent } from './features/devices/device-upsert.component';
import { ShellComponent } from './layout/shell.component';

export const routes: Routes = [
  { path: 'login', canActivate: [guestGuard], component: LoginComponent },
  { path: 'register', canActivate: [guestGuard], component: RegisterComponent },
  {
    path: '',
    canActivate: [authGuard],
    component: ShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'devices' },
      { path: 'devices/new', component: DeviceUpsertComponent },
      { path: 'devices/:id/edit', component: DeviceUpsertComponent },
      { path: 'devices/:id', component: DeviceDetailComponent },
      { path: 'devices', component: DeviceListComponent },
    ],
  },
  { path: '**', redirectTo: '/devices' },
];
