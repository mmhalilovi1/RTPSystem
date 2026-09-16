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

@Injectable({ providedIn: 'root' })
export class PositionService {
  private readonly apiUrl = 'http://localhost:5168/api/positions';

  constructor(private http: HttpClient) { }

  getAllOpen(filter: PositionFilter): Observable<Position[]> {
    let params = new HttpParams();

    if (filter.location) params = params.set('location', filter.location);
    if (filter.employmentType) params = params.set('employmentType', filter.employmentType);
    if (filter.minRequiredExperience != null) {
      params = params.set('minRequiredExperience', filter.minRequiredExperience.toString());
    }

    return this.http.get<Position[]>(`${this.apiUrl}/open`, { params });
  }
}
