import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Position {
  id: string;
  title: string;
  description: string;
  location: string;
  employmentType: string;
  requiredExperience: number;
  status: string;
  deadline: string;
  requiredSkills: string[];
}

export interface PositionFilter {
  location?: string;
  employmentType?: string;
  minRequiredExperience?: number;
}

export interface PositionRequest {
  title: string;
  description: string;
  location: string;
  employmentType: string;
  requiredExperience: number;
  deadline: string;
  requiredSkills: string[];
}

@Injectable({ providedIn: 'root' })
export class PositionService {
  private readonly apiUrl = 'http://localhost:5168/api/positions';

  constructor(private http: HttpClient) { }

  getAllOpen(filter: PositionFilter): Observable<Position[]> {
    return this.http.get<Position[]>(`${this.apiUrl}/open`, { params: this.buildParams(filter) });
  }

  getAll(filter: PositionFilter): Observable<Position[]> {   
    return this.http.get<Position[]>(this.apiUrl, { params: this.buildParams(filter) });
  }

  getById(id: string): Observable<Position> {
    return this.http.get<Position>(`${this.apiUrl}/${id}`);
  }

  create(request: PositionRequest): Observable<Position> {
    return this.http.post<Position>(this.apiUrl, request);
  }

  publish(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/publish`, {});
  }

  close(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/close`, {});
  }

  archive(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/archive`, {});
  }

  private buildParams(filter: PositionFilter): HttpParams {
    let params = new HttpParams();
    if (filter.location) params = params.set('location', filter.location);
    if (filter.employmentType) params = params.set('employmentType', filter.employmentType);
    if (filter.minRequiredExperience != null) {
      params = params.set('minRequiredExperience', filter.minRequiredExperience.toString());
    }
    return params;
  }
}
