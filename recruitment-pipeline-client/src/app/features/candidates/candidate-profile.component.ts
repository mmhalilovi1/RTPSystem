import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Candidate, CandidateService } from '../../core/services/candidate.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-candidate-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './candidate-profile.component.html',
  styleUrl: './candidate-profile.component.css'
})
export class CandidateProfileComponent implements OnInit {
  candidate = signal<Candidate | null>(null);
  isLoading = signal(true);
  hasProfile = signal(false);
  errorMessage = signal<string | null>(null);
  newSkill = signal('');
  resumeUrlInput = signal('');
  private fb = inject(FormBuilder);

  createForm = this.fb.group({
    fullName: ['', Validators.required],
    phoneNumber: ['', Validators.required],
    yearsOfExperience: [0, [Validators.required, Validators.min(0)]],
    skillsInput: ['']
  });

  constructor(private candidateService: CandidateService, private authService: AuthService) { }

  ngOnInit(): void {
    this.candidateService.getMyProfile().subscribe({
      next: (candidate) => {
        this.candidate.set(candidate);
        this.hasProfile.set(true);
        this.isLoading.set(false);
      },
      error: () => {
        this.hasProfile.set(false);
        this.isLoading.set(false);
      }
    });
  }

  onCreateSubmit(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const raw = this.createForm.getRawValue();
    const skills = (raw.skillsInput ?? '').split(',').map(s => s.trim()).filter(s => s.length > 0);

    this.candidateService.create({
      fullName: raw.fullName!,
      phoneNumber: raw.phoneNumber!,
      yearsOfExperience: raw.yearsOfExperience!,
      skills
    }).subscribe({
      next: (candidate) => {
        this.authService.refreshAccessToken().subscribe({
          next: () => {
            this.candidate.set(candidate);
            this.hasProfile.set(true);
          },
          error: () => {
            this.candidate.set(candidate);
            this.hasProfile.set(true);
            this.errorMessage.set('Profil je kreiran. Molimo ulogujte se ponovo prije daljih akcija.');
          }
        });
      },
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Kreiranje profila nije uspjelo.')
    });
  }

  uploadResume(): void {
    const current = this.candidate();
    if (!current || !this.resumeUrlInput()) return;

    this.candidateService.uploadResume(current.id, this.resumeUrlInput()).subscribe({
      next: () => {
        this.candidate.set({ ...current, resumeUrl: this.resumeUrlInput() });
        this.resumeUrlInput.set('');
      },
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Postavljanje CV-a nije uspjelo.')
    });
  }

  addSkill(): void {
    const current = this.candidate();
    const skill = this.newSkill().trim();
    if (!current || !skill) return;

    this.candidateService.addSkill(current.id, skill).subscribe({
      next: () => {
        this.candidate.set({ ...current, skills: [...current.skills, skill] });
        this.newSkill.set('');
      },
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Dodavanje vještine nije uspjelo.')
    });
  }

  removeSkill(skill: string): void {
    const current = this.candidate();
    if (!current) return;

    this.candidateService.removeSkill(current.id, skill).subscribe({
      next: () => {
        this.candidate.set({ ...current, skills: current.skills.filter(s => s !== skill) });
      }
    });
  }
}
