import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.authService.register(this.form.getRawValue() as { email: string; password: string }).subscribe({
      next: () => this.router.navigate(['/positions']),
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(
          err.status === 409 ? 'Email je zauzet' : 'Došlo je do greške. Pokušajte ponovo.'
        ); 
      }
    });
  }
}
