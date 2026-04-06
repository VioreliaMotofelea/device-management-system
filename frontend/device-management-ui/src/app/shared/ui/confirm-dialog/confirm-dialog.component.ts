import { Component, HostListener, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  imports: [],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.css',
})
export class ConfirmDialogComponent {
  open = input(false);
  titleText = input('Confirm');
  message = input('');
  confirmLabel = input('OK');
  cancelLabel = input('Cancel');
  destructive = input(false);
  busy = input(false);

  readonly titleId = `confirm-title-${Math.random().toString(36).slice(2, 11)}`;
  readonly descId = `confirm-desc-${Math.random().toString(36).slice(2, 11)}`;

  confirmed = output<void>();
  cancelled = output<void>();

  @HostListener('document:keydown.escape', ['$event'])
  protected onEscape(ev: Event): void {
    if (!this.open() || this.busy()) {
      return;
    }
    ev.preventDefault();
    this.cancelled.emit();
  }

  protected onBackdropClick(event: MouseEvent): void {
    if (this.busy()) {
      return;
    }
    if (event.target === event.currentTarget) {
      this.cancelled.emit();
    }
  }

  protected onConfirm(): void {
    if (this.busy()) {
      return;
    }
    this.confirmed.emit();
  }

  protected onCancel(): void {
    if (this.busy()) {
      return;
    }
    this.cancelled.emit();
  }
}
