import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  fullName: string;
  email: string;
  token: string;
  expiresAt: string;
}

export interface AuthApiResponse {
  message: string;
  data: AuthResponse;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:7089/api/auth';
  constructor(private http: HttpClient) {}

  register(request: RegisterRequest): Observable<AuthApiResponse> {
    return this.http
      .post<AuthApiResponse>(`${this.apiUrl}/register`, request)
      .pipe(tap((response) => this.saveSession(response.data)));
  }

  login(request: LoginRequest): Observable<AuthApiResponse> {
    return this.http
      .post<AuthApiResponse>(`${this.apiUrl}/login`, request)
      .pipe(tap((response) => this.saveSession(response.data)));
  }

  private saveSession(user: AuthResponse): void {
    localStorage.setItem('token', user.token);
    localStorage.setItem('user', JSON.stringify(user));
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getCurrentUser(): AuthResponse | null {
    const user = localStorage.getItem('user');

    return user ? (JSON.parse(user) as AuthResponse) : null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
