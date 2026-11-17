import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import {
  Transaction,
  TransactionType,
  CreateTransactionRequest,
  UpdateTransactionRequest,
} from '../../../core/services/models/transaction';

import { TransactionService } from '../../../core/services/services/transaction';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CurrencyPipe, DatePipe],
  templateUrl: './transactions.html',
  styleUrl: './transactions.scss',
})
export class Transactions implements OnInit {
  private fb = inject(FormBuilder);
  private transactionService = inject(TransactionService);

  transactions: Transaction[] = [];

  selectedFilter: 'All' | TransactionType = 'All';

  isLoading = false;
  isSaving = false;

  isEditing = false;
  editingId: number | null = null;

  showForm = false;

  successMessage = '';
  errorMessage = '';

  transactionForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    type: ['Expense' as TransactionType, Validators.required],
    category: ['', [Validators.required, Validators.maxLength(50)]],
    transactionDate: [new Date().toISOString().slice(0, 10), Validators.required],
    description: ['', Validators.maxLength(500)],
  });

  ngOnInit(): void {
    this.loadTransactions();
  }

  // Load transactions
  loadTransactions(clearMessages = true): void {
    this.isLoading = true;

    if (clearMessages) {
      this.clearMessages();
    }

    this.transactionService
      .getAllTransactions()
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (response: Transaction[]) => {
          this.transactions = response;
        },
        error: (error: HttpErrorResponse) => {
          this.handleError(error, 'Unable to load transactions.');
        },
      });
  }

  // Filter transactions
  get filteredTransactions(): Transaction[] {
    if (this.selectedFilter === 'All') {
      return this.transactions;
    }

    return this.transactions.filter((transaction) => transaction.type === this.selectedFilter);
  }

  // Total income
  get totalIncome(): number {
    return this.transactions
      .filter((transaction) => transaction.type === 'Income')
      .reduce((total, transaction) => total + transaction.amount, 0);
  }

  // Total expenses
  get totalExpense(): number {
    return this.transactions
      .filter((transaction) => transaction.type === 'Expense')
      .reduce((total, transaction) => total + transaction.amount, 0);
  }

  // Balance
  get balance(): number {
    return this.totalIncome - this.totalExpense;
  }

  // Change filter
  changeFilter(filter: 'All' | TransactionType): void {
    this.selectedFilter = filter;
  }

  // Open create form
  openAddForm(): void {
    this.isEditing = false;
    this.editingId = null;

    this.transactionForm.reset({
      title: '',
      amount: 0,
      type: 'Expense',
      category: '',
      transactionDate: new Date().toISOString().slice(0, 10),
      description: '',
    });

    this.showForm = true;
    this.clearMessages();
  }

  // Open edit form
  editTransaction(transaction: Transaction): void {
    this.isEditing = true;
    this.editingId = transaction.transactionId;

    this.transactionForm.reset({
      title: transaction.title,
      amount: transaction.amount,
      type: transaction.type,
      category: transaction.category,
      transactionDate: this.formatDateForInput(transaction.transactionDate),
      description: transaction.description ?? '',
    });

    this.showForm = true;
    this.clearMessages();
  }

  // Close form
  closeForm(): void {
    this.showForm = false;
    this.isEditing = false;
    this.editingId = null;

    this.transactionForm.reset({
      title: '',
      amount: 0,
      type: 'Expense',
      category: '',
      transactionDate: new Date().toISOString().slice(0, 10),
      description: '',
    });
  }

  // Submit transaction
  submitTransaction(): void {
    if (this.transactionForm.invalid) {
      this.transactionForm.markAllAsTouched();
      return;
    }

    if (this.isSaving) {
      return;
    }

    this.isSaving = true;
    this.clearMessages();

    const formValue = this.transactionForm.getRawValue();

    const request: CreateTransactionRequest = {
      title: formValue.title.trim(),
      amount: Number(formValue.amount),
      type: formValue.type,
      category: formValue.category.trim(),
      transactionDate: `${formValue.transactionDate}T00:00:00`,
      description: formValue.description.trim() || null,
    };

    if (this.isEditing && this.editingId !== null) {
      const updateRequest: UpdateTransactionRequest = request;

      this.transactionService
        .updateTransaction(this.editingId, updateRequest)
        .pipe(
          finalize(() => {
            this.isSaving = false;
          }),
        )
        .subscribe({
          next: () => {
            this.closeForm();
            this.successMessage = 'Transaction updated successfully.';
            this.loadTransactions(false);
          },
          error: (error: HttpErrorResponse) => {
            this.handleError(error, 'Unable to update transaction.');
          },
        });
    } else {
      this.transactionService
        .createTransaction(request)
        .pipe(
          finalize(() => {
            this.isSaving = false;
          }),
        )
        .subscribe({
          next: () => {
            this.closeForm();
            this.successMessage = 'Transaction added successfully.';
            this.loadTransactions(false);
          },
          error: (error: HttpErrorResponse) => {
            this.handleError(error, 'Unable to create transaction.');
          },
        });
    }
  }

  // Delete transaction
  deleteTransaction(transaction: Transaction): void {
    const confirmed = window.confirm(`Are you sure you want to delete "${transaction.title}"?`);

    if (!confirmed) {
      return;
    }

    this.clearMessages();

    this.transactionService.deleteTransaction(transaction.transactionId).subscribe({
      next: () => {
        this.successMessage = 'Transaction deleted successfully.';
        this.loadTransactions(false);
      },
      error: (error: HttpErrorResponse) => {
        this.handleError(error, 'Unable to delete transaction.');
      },
    });
  }

  // Format date
  private formatDateForInput(date: string): string {
    return date.substring(0, 10);
  }

  // Handle API errors
  private handleError(error: HttpErrorResponse, fallbackMessage: string): void {
    if (error.status === 401) {
      this.errorMessage = 'Your session has expired. Please log in again.';
      return;
    }

    if (error.status === 0) {
      this.errorMessage = 'Cannot connect to the backend. Check whether the API is running.';
      return;
    }

    this.errorMessage = error.error?.message ?? error.error?.title ?? fallbackMessage;
  }

  // Clear messages
  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
