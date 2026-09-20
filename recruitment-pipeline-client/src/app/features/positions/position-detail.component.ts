import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Position, PositionService } from '../../core/services/position.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-position-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './position-detail.component.html',
  styleUrl: './position-detail.component.css'
})
export class PositionDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);

  position = signal<Position | null>(null);
  isLoading = signal(false);
  actionMessage = signal<string | null>(null);

  constructor(private positionService: PositionService, public authService: AuthService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) this.loadPosition(id);
    });
  }

  get isPrivileged(): boolean {
    const role = this.authService.role();
    return role === 'Admin' || role === 'Recruiter';
  }

  publish(): void {
    this.runAction(id => this.positionService.publish(id));
  }

  close(): void {
    this.runAction(id => this.positionService.close(id));
  }

  archive(): void {
    this.runAction(id => this.positionService.archive(id));
  }

  private runAction(action: (id: string) => import('rxjs').Observable<void>): void {
    const current = this.position();
    if (!current) return;

    action(current.id).subscribe({
      next: () => this.loadPosition(current.id),
      error: (err) => this.actionMessage.set(err.error?.message ?? 'Akcija nije uspjela.')
    });
  }

  private loadPosition(id: string): void {
    this.isLoading.set(true);
    this.positionService.getById(id).subscribe({
      next: (position) => {
        this.position.set(position);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}
