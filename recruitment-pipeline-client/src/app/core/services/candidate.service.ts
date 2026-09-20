import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Candidate {
  id: string;
  userId: string;
  fullName: string;
  phoneNumber: string;
  yearsOfExperience: number;
  skills: string[];
  resumeUrl: string | null;
  createdAt: string;
}

export interface CandidateRequest {
  fullName: string;
  phoneNumber: string;
  yearsOfExperience: number;
  skills: string[];
}

export interface CandidateUpdateRequest {
  fullName?: string;
  phoneNumber?: string;
  yearsOfExperience?: number;
}

@Injectable({ providedIn: 'root' })
export class CandidateService {
  private readonly apiUrl = 'http://localhost:5168/api/candidates';

  constructor(private http: HttpClient) { }

  getMyProfile(): Observable<Candidate> {
    return this.http.get<Candidate>(`${this.apiUrl}/me`);
  }

  create(request: CandidateRequest): Observable<Candidate> {
    return this.http.post<Candidate>(this.apiUrl, request);
  }

  update(id: string, request: CandidateUpdateRequest): Observable<Candidate> {
    return this.http.patch<Candidate>(`${this.apiUrl}/${id}`, request);
  }

  uploadResume(id: string, resumeUrl: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/resume`, { resumeUrl });
  }

  addSkill(id: string, skillName: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/skills`, { skillName });
  }

  removeSkill(id: string, skillName: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/skills/${encodeURIComponent(skillName)}`);
  }
}
