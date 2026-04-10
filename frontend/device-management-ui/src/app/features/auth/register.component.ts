import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { getApiErrorMessage } from '../../core/http/api-error';
import { MIN_PASSWORD_LENGTH } from '../../core/validation/form-constants';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly minPasswordLength = MIN_PASSWORD_LENGTH;
  protected readonly submitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly form = this.fb.group(
    {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(this.minPasswordLength)]],
      confirmPassword: [
        '',
        [Validators.required, Validators.minLength(this.minPasswordLength)],
      ],
    },
    { validators: RegisterComponent.passwordsMatchValidator },
  );

  protected submit(): void {
    this.errorMessage.set(null);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const { email, password, confirmPassword } = this.form.getRawValue();

    this.auth
      .register({ email: email.trim(), password, confirmPassword })
      .subscribe({
      next: async () => {
        this.submitting.set(false);
        await this.router.navigateByUrl('/devices');
      },
      error: (err) => {
        this.submitting.set(false);
        this.errorMessage.set(
          getApiErrorMessage(err, 'Registration failed. Try again.'),
        );
      },
      });
  }

  private static passwordsMatchValidator(
    control: AbstractControl,
  ): ValidationErrors | null {
    const password = control.get('password')?.value as string | undefined;
    const confirmPassword = control.get('confirmPassword')?.value as
      | string
      | undefined;

    if (!password || !confirmPassword) {
      return null;
    }

    return password === confirmPassword ? null : { passwordMismatch: true };
  }
}
