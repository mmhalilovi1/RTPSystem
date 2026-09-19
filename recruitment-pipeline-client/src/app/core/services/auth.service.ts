import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:5168/api/auth';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => this.storeTokens(response))
    );
  }

  register(request: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, request);
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }

  getUserRole(): string | null {
    const token = this.getAccessToken();
    if (!token) return null;

    try {
      const payloadBase64 = token.split('.')[1];
      const payloadJson = atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/'));
      const payload = JSON.parse(payloadJson);

      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null;
    } catch {
      return null;
    }
  }

  private storeTokens(response: AuthResponse): void {
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);
  }
}
