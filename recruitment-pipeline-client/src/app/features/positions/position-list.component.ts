import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { Position, PositionService } from '../../core/services/position.service';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-position-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './position-list.component.html',
  styleUrl: './position-list.component.css'
})
export class PositionListComponent implements OnInit {
  private fb = inject(FormBuilder);
  positions = signal<Position[]>([]);
  isLoading = signal(false);

  filterForm = this.fb.group({
    location: [''],
    employmentType: [''],
    minRequiredExperience: [null as number | null]
  });

  constructor(
    private positionService: PositionService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadPositions();

    this.filterForm.valueChanges.pipe(debounceTime(400)).subscribe(() => {
      this.loadPositions();
    });
  }  

  private loadPositions(): void {
    this.isLoading.set(true);
    const raw = this.filterForm.getRawValue();

    const role = this.authService.role();
    const isPrivileged = role === 'Admin' || role === 'Recruiter';

    const request$ = isPrivileged
      ? this.positionService.getAll({
        location: raw.location || undefined,
        employmentType: raw.employmentType || undefined,
        minRequiredExperience: raw.minRequiredExperience ?? undefined
      })
      : this.positionService.getAllOpen({
        location: raw.location || undefined,
        employmentType: raw.employmentType || undefined,
        minRequiredExperience: raw.minRequiredExperience ?? undefined
      });

    request$.subscribe({
      next: (positions) => {
        this.positions.set(positions);
        this.isLoading.set(false);
      },
      error: () => { this.isLoading.set(false); }
    });
  }

  viewDetails(positionId: string): void {
    this.router.navigate(['/positions', positionId]);
  }
}
