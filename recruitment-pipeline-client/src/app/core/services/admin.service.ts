import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PendingRecruiter {
  id: string;
  email: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly apiUrl = 'http://localhost:5168/api/auth';

  constructor(private http: HttpClient) { }

  getPendingRecruiters(): Observable<PendingRecruiter[]> {
    return this.http.get<PendingRecruiter[]>(`${this.apiUrl}/pending-recruiters`);
  }

  approveRecruiter(userId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/approve-recruiter/${userId}`, {});
  }

  rejectRecruiter(userId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/reject-recruiter/${userId}`, {});
  }
}
