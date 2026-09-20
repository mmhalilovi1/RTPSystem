import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PositionService } from '../../core/services/position.service';

@Component({
  selector: 'app-position-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './position-form.component.html',
  styleUrl: './position-form.component.css'
})
export class PositionFormComponent {
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);
  private fb = inject(FormBuilder);

  form = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    location: ['', Validators.required],
    employmentType: ['FullTime', Validators.required],
    requiredExperience: [0, [Validators.required, Validators.min(0)]],
    deadline: ['', Validators.required],
    skillsInput: ['']
  });

  constructor(private positionService: PositionService, private router: Router) { }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();
    const skillNames = (raw.skillsInput ?? '')
      .split(',')
      .map(s => s.trim())
      .filter(s => s.length > 0);

    this.positionService.create({
      title: raw.title!,
      description: raw.description!,
      location: raw.location!,
      employmentType: raw.employmentType!,
      requiredExperience: raw.requiredExperience!,
      deadline: new Date(raw.deadline!).toISOString(),
      requiredSkills: skillNames
    }).subscribe({
      next: (position) => this.router.navigate(['/positions', position.id]),
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.message ?? 'Došlo je do greške pri kreiranju pozicije.');
      }
    });
  }
}
