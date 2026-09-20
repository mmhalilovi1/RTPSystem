import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import {
  Application, ApplicationService, InterviewStage, Feedback
} from '../../core/services/application.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-application-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './application-detail.component.html',
  styleUrl: './application-detail.component.css'
})
export class ApplicationDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);

  application = signal<Application | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  newStageType = signal('Screening');
  stageTypes = ['Screening', 'TechnicalInterview', 'HrInterview', 'FinalDecision'];

  scheduleDateByStage = signal<Record<string, string>>({});
  completeOutcomeByStage = signal<Record<string, string>>({});
  completeNotesByStage = signal<Record<string, string>>({});

  feedbackByStage = signal<Record<string, Feedback[]>>({});
  feedbackRatingByStage = signal<Record<string, number>>({});
  feedbackCommentsByStage = signal<Record<string, string>>({});

  constructor(private applicationService: ApplicationService, public authService: AuthService) { }

  get isPrivileged(): boolean {
    const role = this.authService.role();
    return role === 'Admin' || role === 'Recruiter';
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.load(id);
  }

  addStage(): void {
    const app = this.application();
    if (!app) return;

    this.applicationService.addInterviewStage(app.id, this.newStageType()).subscribe({
      next: () => this.load(app.id),
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Dodavanje faze nije uspjelo.')
    });
  }

  scheduleStage(stageId: string): void {
    const app = this.application();
    const date = this.scheduleDateByStage()[stageId];
    if (!app || !date) return;

    this.applicationService.scheduleInterviewStage(app.id, stageId, new Date(date).toISOString()).subscribe({
      next: () => this.load(app.id),
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Zakazivanje nije uspjelo.')
    });
  }

  completeStage(stageId: string): void {
    const app = this.application();
    const outcome = this.completeOutcomeByStage()[stageId];
    const notes = this.completeNotesByStage()[stageId] ?? null;
    if (!app || !outcome) return;

    this.applicationService.completeInterviewStage(app.id, stageId, outcome, notes).subscribe({
      next: () => this.load(app.id),
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Završavanje faze nije uspjelo.')
    });
  }

  loadFeedback(stageId: string): void {
    const app = this.application();
    if (!app) return;

    this.applicationService.getFeedbackForStage(app.id, stageId).subscribe({
      next: (list) => this.feedbackByStage.update(map => ({ ...map, [stageId]: list }))
    });
  }

  addFeedback(stageId: string): void {
    const app = this.application();
    const rating = this.feedbackRatingByStage()[stageId];
    const comments = this.feedbackCommentsByStage()[stageId];
    if (!app || !rating || !comments) return;

    this.applicationService.addFeedback(app.id, stageId, rating, comments).subscribe({
      next: () => {
        this.loadFeedback(stageId);
        this.feedbackCommentsByStage.update(map => ({ ...map, [stageId]: '' }));
      },
      error: (err) => this.errorMessage.set(err.error?.message ?? 'Dodavanje feedback-a nije uspjelo.')
    });
  }

  setScheduleDate(stageId: string, value: string): void {
    this.scheduleDateByStage.update(map => ({ ...map, [stageId]: value }));
  }

  setCompleteOutcome(stageId: string, value: string): void {
    this.completeOutcomeByStage.update(map => ({ ...map, [stageId]: value }));
  }

  setCompleteNotes(stageId: string, value: string): void {
    this.completeNotesByStage.update(map => ({ ...map, [stageId]: value }));
  }

  setFeedbackRating(stageId: string, value: number): void {
    this.feedbackRatingByStage.update(map => ({ ...map, [stageId]: value }));
  }

  setFeedbackComments(stageId: string, value: string): void {
    this.feedbackCommentsByStage.update(map => ({ ...map, [stageId]: value }));
  }

  private load(id: string): void {
    this.isLoading.set(true);
    this.applicationService.getById(id).subscribe({
      next: (app) => {
        this.application.set(app);
        this.isLoading.set(false);
        if (this.isPrivileged) {
          app.interviewStages
            .filter(s => s.outcome !== 'Pending')
            .forEach(s => this.loadFeedback(s.id));
        }
      },
      error: () => this.isLoading.set(false)
    });
  }
}
