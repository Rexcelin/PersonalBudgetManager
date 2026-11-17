import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import {
  Budget,
  CreateBudgetRequest,
  UpdateBudgetRequest,
} from '../../../core/services/models/budget';
import { BudgetService } from '../../../core/services/services/budget';

@Component({
  selector: 'app-budgets',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CurrencyPipe],
  templateUrl: './budgets.html',
  styleUrl: './budgets.scss',
})
export class Budgets implements OnInit {
  private fb = inject(FormBuilder);
  private budgetService = inject(BudgetService);

  budgets: Budget[] = [];

  isLoading = false;
  isSaving = false;
  isDeleting = false;

  showForm = false;
  isEditing = false;
  editingId: number | null = null;

  successMessage = '';
  errorMessage = '';

  readonly months = [
    { value: 1, name: 'January' },
    { value: 2, name: 'February' },
    { value: 3, name: 'March' },
    { value: 4, name: 'April' },
    { value: 5, name: 'May' },
    { value: 6, name: 'June' },
    { value: 7, name: 'July' },
    { value: 8, name: 'August' },
    { value: 9, name: 'September' },
    { value: 10, name: 'October' },
    { value: 11, name: 'November' },
    { value: 12, name: 'December' },
  ];

  readonly currentYear = new Date().getFullYear();

  readonly years = Array.from({ length: 11 }, (_, index) => this.currentYear - 5 + index);

  budgetForm = this.fb.nonNullable.group({
    monthlyLimit: [0, [Validators.required, Validators.min(0.01)]],
    month: [
      new Date().getMonth() + 1,
      [Validators.required, Validators.min(1), Validators.max(12)],
    ],
    year: [this.currentYear, [Validators.required, Validators.min(2000), Validators.max(2100)]],
  });

  ngOnInit(): void {
    this.loadBudgets();
  }

  // Load all budgets
  loadBudgets(clearMessages = true): void {
    this.isLoading = true;

    if (clearMessages) {
      this.clearMessages();
    }

    this.budgetService
      .getAllBudgets()
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (response: Budget[]) => {
          this.budgets = response;
        },
        error: (error: HttpErrorResponse) => {
          this.handleError(error, 'Unable to load budgets.');
        },
      });
  }

  // Get month name
  getMonthName(month: number): string {
    return this.months.find((m) => m.value === month)?.name ?? '';
  }

  // Total number of budgets
  get totalBudgets(): number {
    return this.budgets.length;
  }

  // Total of the limits currently listed
  get totalBudgetAmount(): number {
    return this.budgets.reduce((total, budget) => total + budget.monthlyLimit, 0);
  }

  // Open create form
  openAddForm(): void {
    this.isEditing = false;
    this.editingId = null;

    this.budgetForm.enable();

    this.budgetForm.reset({
      monthlyLimit: 0,
      month: new Date().getMonth() + 1,
      year: this.currentYear,
    });

    this.showForm = true;
    this.clearMessages();
  }

  // Open edit form
  editBudget(budget: Budget): void {
    this.isEditing = true;
    this.editingId = budget.budgetId;

    this.budgetForm.enable();

    this.budgetForm.reset({
      monthlyLimit: budget.monthlyLimit,
      month: budget.month,
      year: budget.year,
    });

    // Month and year cannot be changed while editing
    this.budgetForm.controls.month.disable();
    this.budgetForm.controls.year.disable();

    this.showForm = true;
    this.clearMessages();
  }

  // Close form
  closeForm(): void {
    this.showForm = false;
    this.isEditing = false;
    this.editingId = null;

    this.budgetForm.enable();

    this.budgetForm.reset({
      monthlyLimit: 0,
      month: new Date().getMonth() + 1,
      year: this.currentYear,
    });
  }

  // Create or update budget
  submitBudget(): void {
    if (this.budgetForm.invalid) {
      this.budgetForm.markAllAsTouched();
      return;
    }

    if (this.isSaving) {
      return;
    }

    this.isSaving = true;
    this.clearMessages();

    const formValue = this.budgetForm.getRawValue();

    if (this.isEditing && this.editingId !== null) {
      const updateRequest: UpdateBudgetRequest = {
        monthlyLimit: Number(formValue.monthlyLimit),
      };

      this.budgetService
        .updateBudget(this.editingId, updateRequest)
        .pipe(
          finalize(() => {
            this.isSaving = false;
          }),
        )
        .subscribe({
          next: () => {
            this.closeForm();

            this.successMessage = 'Budget updated successfully.';

            this.loadBudgets(false);
          },
          error: (error: HttpErrorResponse) => {
            this.handleError(error, 'Unable to update budget.');
          },
        });
    } else {
      const createRequest: CreateBudgetRequest = {
        monthlyLimit: Number(formValue.monthlyLimit),
        month: Number(formValue.month),
        year: Number(formValue.year),
      };

      this.budgetService
        .createBudget(createRequest)
        .pipe(
          finalize(() => {
            this.isSaving = false;
          }),
        )
        .subscribe({
          next: () => {
            this.closeForm();

            this.successMessage = 'Budget created successfully.';

            this.loadBudgets(false);
          },
          error: (error: HttpErrorResponse) => {
            this.handleError(error, 'Unable to create budget.');
          },
        });
    }
  }

  // Delete budget
  deleteBudget(budget: Budget): void {
    const confirmed = window.confirm(
      `Are you sure you want to delete the ${this.getMonthName(budget.month)} ${budget.year} budget?`,
    );

    if (!confirmed) {
      return;
    }

    if (this.isDeleting) {
      return;
    }

    this.isDeleting = true;
    this.clearMessages();

    this.budgetService
      .deleteBudget(budget.budgetId)
      .pipe(
        finalize(() => {
          this.isDeleting = false;
        }),
      )
      .subscribe({
        next: () => {
          this.successMessage = 'Budget deleted successfully.';

          this.loadBudgets(false);
        },
        error: (error: HttpErrorResponse) => {
          this.handleError(error, 'Unable to delete budget.');
        },
      });
  }

  // Error handling
  private handleError(error: HttpErrorResponse, fallbackMessage: string): void {
    if (error.status === 401) {
      this.errorMessage = 'Your session has expired. Please log in again.';
      return;
    }

    if (error.status === 403) {
      this.errorMessage = 'You are not authorized to perform this action.';
      return;
    }

    if (error.status === 0) {
      this.errorMessage = 'Cannot connect to the backend. Check whether the API is running.';
      return;
    }

    this.errorMessage = error.error?.message ?? error.error?.title ?? fallbackMessage;
  }

  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
