import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs';

import {
  Transaction,
  CreateTransactionRequest,
  UpdateTransactionRequest,
} from '../models/transaction';

import { AuthService } from '../auth';
@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private apiUrl = 'https://localhost:7089/api/transactions';

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();

    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  // Get all transactions
  getAllTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // Get transaction by ID
  getTransactionById(id: number): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // Create transaction
  createTransaction(transaction: CreateTransactionRequest): Observable<Transaction> {
    return this.http.post<Transaction>(this.apiUrl, transaction, { headers: this.getHeaders() });
  }

  // Update transaction
  updateTransaction(id: number, transaction: UpdateTransactionRequest): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/${id}`, transaction, {
      headers: this.getHeaders(),
    });
  }

  // Delete transaction
  deleteTransaction(id: number): Observable<unknown> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
