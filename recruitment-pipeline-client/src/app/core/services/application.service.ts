import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface InterviewStage {
  id: string;
  stageType: string;
  outcome: string;
  scheduledAt: string | null;
  completedAt: string | null;
  notes: string | null;
}

export interface Application {
  id: string;
  positionId: string;
  candidateId: string;
  status: string;
  appliedAt: string;
  decisionAt: string | null;
  interviewStages: InterviewStage[];
  positionTitle: string | null;
  candidateFullName: string | null;
}

export interface Feedback {
  id: string;
  interviewStageId: string;
  authorUserId: string;
  rating: number;
  comments: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private readonly apiUrl = 'http://localhost:5168/api/applications';

  constructor(private http: HttpClient) { }

  submit(positionId: string): Observable<Application> {
    return this.http.post<Application>(this.apiUrl, { positionId });
  }

  getById(id: string): Observable<Application> {
    return this.http.get<Application>(`${this.apiUrl}/${id}`);
  }

  getMyApplications(): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.apiUrl}/my`);
  }

  getByPosition(positionId: string): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.apiUrl}/by-position/${positionId}`);
  }

  addInterviewStage(applicationId: string, stageType: string): Observable<InterviewStage> {
    return this.http.post<InterviewStage>(`${this.apiUrl}/${applicationId}/interview-stages`, { stageType });
  }

  scheduleInterviewStage(applicationId: string, stageId: string, scheduledAt: string): Observable<InterviewStage> {
    return this.http.patch<InterviewStage>(
      `${this.apiUrl}/${applicationId}/interview-stages/${stageId}/schedule`,
      { scheduledAt }
    );
  }

  completeInterviewStage(applicationId: string, stageId: string, outcome: string, notes: string | null): Observable<InterviewStage> {
    return this.http.patch<InterviewStage>(
      `${this.apiUrl}/${applicationId}/interview-stages/${stageId}/complete`,
      { outcome, notes }
    );
  }

  addFeedback(applicationId: string, stageId: string, rating: number, comments: string): Observable<Feedback> {
    return this.http.post<Feedback>(
      `${this.apiUrl}/${applicationId}/interview-stages/${stageId}/feedback`,
      { rating, comments }
    );
  }

  getFeedbackForStage(applicationId: string, stageId: string): Observable<Feedback[]> {
    return this.http.get<Feedback[]>(`${this.apiUrl}/${applicationId}/interview-stages/${stageId}/feedback`);
  }
}
