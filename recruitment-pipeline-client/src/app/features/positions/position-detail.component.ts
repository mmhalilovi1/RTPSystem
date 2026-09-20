import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Position, PositionService } from '../../core/services/position.service';
import { AuthService } from '../../core/services/auth.service';
import { Application, ApplicationService } from '../../core/services/application.service';

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
  applications = signal<Application[]>([]);
  applyMessage = signal<string | null>(null);

  constructor(
    private positionService: PositionService,
    public authService: AuthService,
    private applicationService: ApplicationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadPosition(id);
        if (this.isPrivileged) this.loadApplications(id);
      }
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

  apply(): void {
    const current = this.position();
    if (!current) return;

    this.applyMessage.set(null);
    this.applicationService.submit(current.id).subscribe({
      next: () => this.applyMessage.set('Prijava je uspješno poslana.'),
      error: (err) => this.applyMessage.set(err.error?.message ?? 'Prijava nije uspjela.')
    });
  }

  viewApplication(applicationId: string): void {
    this.router.navigate(['/applications', applicationId]);
  }

  private loadApplications(positionId: string): void {
    this.applicationService.getByPosition(positionId).subscribe({
      next: (apps) => this.applications.set(apps)
    });
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
