import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs';

import { Budget, CreateBudgetRequest, UpdateBudgetRequest } from '../models/budget';

import { AuthService } from '../auth';

@Injectable({
  providedIn: 'root',
})
export class BudgetService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = 'https://localhost:7089/api/budgets';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();

    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });
  }

  // GET: api/budgets
  getAllBudgets(): Observable<Budget[]> {
    return this.http.get<Budget[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // GET: api/budgets/{id}
  getBudgetById(id: number): Observable<Budget> {
    return this.http.get<Budget>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // POST: api/budgets
  createBudget(request: CreateBudgetRequest): Observable<Budget> {
    return this.http.post<Budget>(this.apiUrl, request, { headers: this.getHeaders() });
  }

  // PUT: api/budgets/{id}
  updateBudget(id: number, request: UpdateBudgetRequest): Observable<Budget> {
    return this.http.put<Budget>(`${this.apiUrl}/${id}`, request, { headers: this.getHeaders() });
  }

  // DELETE: api/budgets/{id}
  deleteBudget(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
