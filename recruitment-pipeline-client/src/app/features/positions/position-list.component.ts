import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { Position, PositionService } from '../../core/services/position.service';

@Component({
  selector: 'app-position-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './position-list.component.html',
  styleUrl: './position-list.component.css'
})
export class PositionListComponent implements OnInit {
  private fb = inject(FormBuilder);
  positions: Position[] = [];
  isLoading = false;

  filterForm = this.fb.group({
    location: [''],
    employmentType: [''],
    minRequiredExperience: [null as number | null]
  });

  constructor(private positionService: PositionService) { }

  ngOnInit(): void {
    this.loadPositions();

    this.filterForm.valueChanges.pipe(debounceTime(400)).subscribe(() => {
      this.loadPositions();
    });
  }

  private loadPositions(): void {
    this.isLoading = true;
    const raw = this.filterForm.getRawValue();

    this.positionService.getAllOpen({
      location: raw.location || undefined,
      employmentType: raw.employmentType || undefined,
      minRequiredExperience: raw.minRequiredExperience ?? undefined
    }).subscribe({
      next: (positions) => {
        this.positions = positions;
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }
}
